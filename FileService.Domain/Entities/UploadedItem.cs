using Zack.DomainCommons.Models;

namespace FileService.Domain.Entities;

/// <summary>
/// 文件上传记录项
/// </summary>
public record UploadedItem : BaseEntity, IHasCreationTime
{
    private UploadedItem() { } // 供 EFCore 使用
    public UploadedItem(string fileName, long fileSizeInBytes, string fileSHA256Hash, Uri backupUrl, Uri removeUrl)
    {
        // 在实例化 UploadedItem 对象时，BaseEntity 类（父类）的构造函数会先被调用，以确保父类部分被正确初始化，
        // 然后才是 UploadedItem 类（子类）的构造函数执行其特有的初始化逻辑。
        // 因此 Id 已经在 BaseEntity 类构造的时候赋值了。

        CreationTime = DateTime.Now;
        FileName = fileName;
        FileSizeInBytes = fileSizeInBytes;
        FileSHA256Hash = fileSHA256Hash;
        BackupUrl = backupUrl;
        RemoveUrl = removeUrl;
    }
    
    public DateTime CreationTime { get; private set; }

    /// <summary>
    /// 文件名(包括扩展名)
    /// </summary>
    public string FileName { get; private set; } = null!;

    /// <summary>
    /// 文件大小（尺寸为字节）
    /// </summary>
    public long FileSizeInBytes { get; private set; }

    /// <summary>
    /// 文件散列值（sha256）；
    /// 两个文件的大小和散列值（sha256）都相同的概率非常小，因此只要大小和SHA256相同，就认为是相同的文件。
    /// </summary>
    public string FileSHA256Hash { get; private set; } = null!;

    /// <summary>
    /// 备份文件路径；
    /// 备份文件一般放到内网的高速、稳定设备上，比如NAS等。
    /// </summary>
    public Uri BackupUrl { get; private set; } = null!;

    /// <summary>
    /// 上传的文件供外部访问者访问的路径
    /// </summary>
    public Uri RemoveUrl { get; private set; } = null!;
}