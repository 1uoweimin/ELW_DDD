namespace FileService.Domain;

/// <summary>
/// 防腐层接口（屏蔽这些存储服务器的差异）
/// </summary>
public interface IFSStorage
{
    StorageType StorageType { get; }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="key">一般是文件路径的一部分</param>
    /// <param name="fileStream">文件流</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存储返回的可以被访问的文件Url</returns>
    Task<Uri> SaveAsync(string key, Stream fileStream, CancellationToken cancellationToken = default);
}

/// <summary>
/// 存储服务器类型
/// </summary>
public enum StorageType { Public, Backup }