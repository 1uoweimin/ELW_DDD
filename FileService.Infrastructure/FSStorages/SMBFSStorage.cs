using FileService.Domain;
using FileService.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.FSStorages;

/// <summary>
/// SMS备份服务器；
/// 把本地磁盘作为目标文件夹，仅供开发、演示阶段使用，
/// 在生产环境一定要用专门的云存储服务器，并且不能允许外网访问这台存储服务器。
/// </summary>
public class SMBFSStorage : IFSStorage
{
    private readonly IOptionsSnapshot<SMBFSStorageOptions> _smbOptions;
    public SMBFSStorage(IOptionsSnapshot<SMBFSStorageOptions> smbOptions)
    {
        _smbOptions = smbOptions;
    }

    public StorageType StorageType => StorageType.Backup;

    public async Task<Uri> SaveAsync(string key, Stream fileStream, CancellationToken cancellationToken = default)
    {
        if (key.StartsWith("/"))
            throw new ArgumentException("key shoud not start with /", nameof(key));

        string workingDir = _smbOptions.Value.WorkingDir;
        string fullPath = Path.Combine(workingDir, key);
        string fullDir = Path.GetDirectoryName(fullPath)!;

        if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);
        if (File.Exists(fullPath)) File.Delete(fullPath);

        using Stream stream = File.OpenWrite(fullPath);
        await fileStream.CopyToAsync(stream, cancellationToken);
        return new Uri(fullPath);
    }
}