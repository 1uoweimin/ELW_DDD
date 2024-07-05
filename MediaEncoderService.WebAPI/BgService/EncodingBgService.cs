using CommonInitializer.Options;
using FileService.SDK.NETCore;
using MediaEncoderService.Domain;
using MediaEncoderService.Domain.Entities;
using MediaEncoderService.Infrastructure;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Net;
using Zack.Commons;
using Zack.EventBus;
using Zack.JWT;

namespace MediaEncoderService.WebAPI.BgService;

public class EncodingBgService : BackgroundService
{
    private readonly IServiceScope _serviceScope;
    private readonly ILogger<EncodingBgService> _logger;
    private readonly MediaEncoderDbContext _dbCtx;
    private readonly IEventBus _eventBus;
    private readonly List<RedLockMultiplexer> redLockMultiplexers;
    private readonly IMediaEncoderRepository _repository;
    private readonly MediaEncoderFactory _encoderFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenService _tokenService;
    private readonly IOptionsSnapshot<FileServiceOptions> _fileServiceOpts;
    private readonly IOptionsSnapshot<JWTOptions> _jwtOpts;
    public EncodingBgService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScope = serviceScopeFactory.CreateScope();
        var serviceProvider = _serviceScope.ServiceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<EncodingBgService>>();
        _dbCtx = serviceProvider.GetRequiredService<MediaEncoderDbContext>();
        _eventBus = serviceProvider.GetRequiredService<IEventBus>();
        IConnectionMultiplexer connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
        redLockMultiplexers = new List<RedLockMultiplexer>() { new RedLockMultiplexer(connectionMultiplexer) };
        _repository = serviceProvider.GetRequiredService<IMediaEncoderRepository>();
        _encoderFactory = serviceProvider.GetRequiredService<MediaEncoderFactory>();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        _tokenService = serviceProvider.GetRequiredService<ITokenService>();
        _fileServiceOpts = serviceProvider.GetRequiredService<IOptionsSnapshot<FileServiceOptions>>();
        _jwtOpts = serviceProvider.GetRequiredService<IOptionsSnapshot<JWTOptions>>();
    }

    /// <summary>
    /// Redis分布式锁来避免两个转码服务器处理同一个转码任务的问题（用RedLock分布式锁，锁定对EncodingItem的访问）
    /// </summary>
    /// <param name="encodingItem"></param>
    /// <returns></returns>
    private async Task<bool> RedLockAsync(string id)
    {
        using var redLockFactory = RedLockFactory.Create(redLockMultiplexers);
        string redLockKey = $"MediaEncoder.EncodingItem.{id}";
        using var redLock = await redLockFactory.CreateLockAsync(redLockKey, TimeSpan.FromSeconds(30));
        if (!redLock.IsAcquired)
        {
            //获得失败，资源已经被锁定，说明这个任务被别的实例处理了（有可能有服务器集群来分担转码压力）
            _logger.LogWarning($"获取资源失败（Key={redLockKey}），资源已被锁定");
        }
        return redLock.IsAcquired;
    }

    /// <summary>
    /// 创建临时存储 srcFileInfo 和 destFileInfo 的文件夹，并把路径返回
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private string BuildTempDir(Guid id)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "ELW_DDD_MediaEncodingDir");
        tempDir = Path.Combine(tempDir, id.ToString());
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    /// <summary>
    /// 下载源文件
    /// </summary>
    /// <param name="encItem"></param>
    /// <param name="tempDirPath"></param>
    /// <param name="stopToken"></param>
    /// <returns></returns>
    private async Task<(bool ok, FileInfo)> DownSrcFileInfoAsync(EncodingItem encItem, string tempDirPath, CancellationToken stopToken)
    {
        var srcFullPath = Path.Combine(tempDirPath, encItem.FileName);

        FileInfo srcFileInfo = new FileInfo(srcFullPath);
        srcFileInfo.Directory?.Create();

        _logger.LogInformation($"Id={encItem.Id}，准备从{encItem.SourceUrl}下载到{srcFullPath}");
        using var httpClient = _httpClientFactory.CreateClient();
        var statusCode = await httpClient.DownloadFileAsync(encItem.SourceUrl, srcFullPath, stopToken);
        if (statusCode != HttpStatusCode.OK)
        {
            _logger.LogError($"Id={encItem.Id}，从{encItem.SourceUrl}下载到{srcFullPath}失败，statusCode={statusCode}");
            Directory.Delete(tempDirPath, true);
            return (false, srcFileInfo);
        }
        else
        {
            _logger.LogInformation($"Id={encItem.Id}，从{encItem.SourceUrl}下载到{srcFullPath}成功");
            return (true, srcFileInfo);
        }
    }

    /// <summary>
    /// 构建目标文件
    /// </summary>
    /// <param name="encItem"></param>
    /// <param name="tempDirPath"></param>
    /// <returns></returns>
    private FileInfo BuildDestFileInfo(EncodingItem encItem, string tempDirPath)
    {
        string destFullPath = Path.Combine(tempDirPath, $"{Path.GetFileNameWithoutExtension(encItem.FileName)}_encode.{encItem.OutputFormat}");
        return new FileInfo(destFullPath);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="destFileInfo"></param>
    /// <param name="stopToken"></param>
    /// <returns></returns>
    private async Task<Uri> UploadeFiledAsync(FileInfo destFileInfo, CancellationToken stopToken)
    {
        Uri webRootUrl = _fileServiceOpts.Value.UrlRoot;
        FileServiceClient fileServiceClient = new FileServiceClient(_httpClientFactory, webRootUrl, _tokenService, _jwtOpts.Value);
        return await fileServiceClient.UploadAsync(destFileInfo, stopToken);
    }

    /// <summary>
    /// 计算哈希值
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    private static string ComputeSHA256Hash(FileInfo fileInfo)
    {
        using var stream = fileInfo.OpenRead();
        return HashHelper.ComputeSha256Hash(stream);
    }

    /// <summary>
    /// 转码
    /// </summary>
    /// <param name="srcFileInfo"></param>
    /// <param name="destFileInfo"></param>
    /// <param name="outputFormat"></param>
    /// <param name="stopToken"></param>
    /// <returns></returns>
    private async Task<bool> EncodeAsync(FileInfo srcFileInfo, FileInfo destFileInfo, string outputFormat, CancellationToken stopToken)
    {
        IMediaEncoder? encoder = _encoderFactory.GetMediaEncoder(outputFormat);
        if (encoder == null)
        {
            _logger.LogError($"转码失败，找不到转码器，目标格式:{outputFormat}");
            return false;
        }
        try
        {
            await encoder.EncodeAsync(srcFileInfo, destFileInfo, outputFormat, null, stopToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"转码失败", ex);
            return false;
        }
        return true;

    }

    private async Task ProcessItemAsync(EncodingItem encItem, CancellationToken stopToken)
    {
        _logger.LogInformation($"准备处理 {encItem.Id}");

        // 用RedLock分布式锁，锁定对EncodingItem的访问
        bool acquired = await RedLockAsync(encItem.Id.ToString());
        if (!acquired) return;

        // 开始处理
        encItem.Start();
        await _dbCtx.SaveChangesAsync(stopToken);
        _logger.LogInformation($"开始处理 {encItem.Id}");

        // 创建临时存储 srcFileInfo 和 destFileInfo 的文件夹
        string tempDirPath = BuildTempDir(encItem.Id);

        // 下载源文件，且计算大小和哈希值
        (bool fileDownOk, FileInfo srcFileInfo) = await DownSrcFileInfoAsync(encItem, tempDirPath, stopToken);
        if (!fileDownOk)
        {
            encItem.Fail("下载失败");
            await _dbCtx.SaveChangesAsync(stopToken);
            return;
        }
        long fileSize = srcFileInfo.Length;
        string fileHash = ComputeSHA256Hash(srcFileInfo);

        // 构建目标文件
        FileInfo destFileInfo = BuildDestFileInfo(encItem, tempDirPath);

        try
        {
            // 检查是否存在大小和哈希值一致且已经转码完成的任务
            var preInstance = await _repository.FindCompletedOneAsync(fileSize, fileHash);
            if (preInstance != null)
            {
                _logger.LogInformation($"Id={encItem.Id}，检查存发现在大小和Hash值一致的旧任务Id={preInstance.Id}");
                _eventBus.Publish("MediaEncoding.Duplicated", new { encItem.Id, encItem.OutputUrl, encItem.SourceSystem });
                encItem.Complete(preInstance.OutputUrl!);
                await _dbCtx.SaveChangesAsync();
                return;
            }

            // 开始转码
            _logger.LogInformation($"Id={encItem.Id}转码开始 => 源路径：{srcFileInfo}  目标路径：{destFileInfo}");
            var encodingOk = await EncodeAsync(srcFileInfo, destFileInfo, encItem.OutputFormat, stopToken);
            if (!encodingOk)
            {
                encItem.Fail("转码失败");
                await _dbCtx.SaveChangesAsync();
                return;
            }

            // 开始上传
            _logger.LogInformation($"Id={encItem.Id}转码成功，开始准备上传到");
            Uri destUrl = await UploadeFiledAsync(destFileInfo, stopToken);
            encItem.Complete(destUrl);
            encItem.SetFile(fileSize, fileHash);
            await _dbCtx.SaveChangesAsync();
            _logger.LogInformation($"Id={encItem.Id}转码结果上传成功");
        }
        finally
        {
            Directory.Delete(tempDirPath, true);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        while (!stopToken.IsCancellationRequested)
        {
            var encodingItems = await _repository.FindAsync(ItemStatus.Ready);
            foreach (var encodingItem in encodingItems)
            {
                try
                {
                    //因为转码比较消耗cpu等资源，因此串行转码
                    await ProcessItemAsync(encodingItem, stopToken);
                }
                catch (Exception ex)
                {
                    encodingItem.Fail(ex);
                    await _dbCtx.SaveChangesAsync();
                }
            }
            // 暂停5s，避免没有任务的时候CPU空转
            await Task.Delay(5000);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _serviceScope.Dispose();
    }
}