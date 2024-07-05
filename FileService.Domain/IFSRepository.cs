using FileService.Domain.Entities;

namespace FileService.Domain;

/// <summary>
/// 仓储接口
/// </summary>
public interface IFSRepository
{
    /// <summary>
    /// 查找已经上传的相同大小以及散列值的文件记录
    /// </summary>
    /// <param name="fileSizeInBytes">文件大小</param>
    /// <param name="fileSHA256Hash">文件散列值</param>
    /// <returns>已经上传的文件记录（UploadedItem）</returns>
    Task<UploadedItem?> FindFileAsync(long fileSizeInBytes, string fileSHA256Hash);
}