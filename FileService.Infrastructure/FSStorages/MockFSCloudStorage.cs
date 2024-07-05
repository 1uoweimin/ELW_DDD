using CommonInitializer.Options;
using FileService.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.FSStorages;

/// <summary>
/// 模拟文件云存储服务器；
/// 把文件服务器当成一个云存储服务器，仅供开发、演示阶段使用，
/// 在生产环境一定要用专门的云存储服务器。
/// </summary>
public class MockFSCloudStorage : IFSStorage
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IOptions<FileServiceOptions> _fileServiceOption;
    public MockFSCloudStorage(IWebHostEnvironment webHostEnv, IOptions<FileServiceOptions> fileServiceOption)
    {
        _webHostEnvironment = webHostEnv;
        _fileServiceOption = fileServiceOption;
    }

    public StorageType StorageType => StorageType.Public;

    public async Task<Uri> SaveAsync(string key, Stream fileStream, CancellationToken cancellationToken = default)
    {
        if (key.StartsWith("/"))
            throw new ArgumentException("key shoud not start with /", nameof(key));

        string workingDir = _webHostEnvironment.WebRootPath;
        string fullPath = Path.Combine(workingDir, key);
        string fullDir = Path.GetDirectoryName(fullPath)!;

        if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);
        if (File.Exists(fullPath)) File.Delete(fullPath);

        using Stream stream = File.OpenWrite(fullPath);
        await fileStream.CopyToAsync(stream, cancellationToken);

        Uri baseUrl = _fileServiceOption.Value.UrlRoot;
        return new Uri(baseUrl, key);
    }
}