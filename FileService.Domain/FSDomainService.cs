using FileService.Domain.Entities;
using Zack.Commons;

namespace FileService.Domain;

/// <summary>
/// 文件领域层：领域服务只有抽象的业务逻辑。
/// </summary>
public class FSDomainService
{
    private readonly IFSRepository _fsRepository; //仓储服务
    private readonly IFSStorage _backupStorage; //文件备份服务
    private readonly IFSStorage _removeStorage; //文件存储服务
    public FSDomainService(IFSRepository fsRepository, IEnumerable<IFSStorage> fsStorages)
    {
        _fsRepository = fsRepository;
        _backupStorage = fsStorages.First(f => f.StorageType == StorageType.Backup);
        _removeStorage = fsStorages.First(f => f.StorageType == StorageType.Public);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="fileName">文件名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>existed=false表示当前文件第一次上传，并返回当前上传的文件记录；existed=true表示当前文件之前已经上传过了，并返回之前上传的文件记录。</returns>
    public async Task<(bool existed, UploadedItem uploadedItem)> UploadAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        long fileSizeInBytes = fileStream.Length;
        string fileSHA256Hash = HashHelper.ComputeSha256Hash(fileStream);

        //虽然前端可能已经调用FileExists接口检查过了，但是前端也可能跳过了，或者有并发上传等问题，所以这里再检查一遍。
        UploadedItem? uploadedItem = await _fsRepository.FindFileAsync(fileSizeInBytes, fileSHA256Hash);
        if (uploadedItem != null) return (true, uploadedItem);

        DateTime dt = DateTime.Today;
        string key = $"{dt.Year}/{dt.Month}/{dt.Day}/{fileSHA256Hash}/{fileName}";

        //保存到本地备份或文件存储服务器
        fileStream.Position = 0;
        Uri backupUrl = await _backupStorage.SaveAsync(key, fileStream, cancellationToken);
        fileStream.Position = 0;
        Uri removeUrl = await _removeStorage.SaveAsync(key, fileStream, cancellationToken);
        fileStream.Position = 0;

        //文件上传记录
        uploadedItem = new UploadedItem(fileName, fileSizeInBytes, fileSHA256Hash, backupUrl, removeUrl);

        return (false, uploadedItem);
    }
}