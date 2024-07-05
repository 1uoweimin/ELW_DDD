using MediaEncoderService.Domain.Events;
using Zack.DomainCommons.Models;

namespace MediaEncoderService.Domain.Entities;

public record EncodingItem : BaseEntity, IAggregateRoot, IHasCreationTime
{
    private EncodingItem() { } // 供EFCore使用
    public EncodingItem(Guid id, string fileName, string sourceSystem, Uri sourceUrl, string outputFormat)
    {
        Id = id;
        FileName = fileName;
        CreationTime = DateTime.Now;
        SourceSystem = sourceSystem;
        SourceUrl = sourceUrl;
        OutputFormat = outputFormat;
        Status = ItemStatus.Ready;
        AddDomainEvent(new EncodingItemCreatedEvent(this));
    }

    public DateTime CreationTime { get; init; }

    /// <summary>
    /// 文件名称（文件名和扩展名）
    /// </summary>
    public string FileName { get; private set; } = null!;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long? FileSizeInBytes { get; private set; }

    /// <summary>
    /// 文件散列值（SHA256Hash）
    /// </summary>
    public string? FileSHA256Hash { get; private set; }

    /// <summary>
    /// 源系统名称（需要转码的源系统名称）
    /// </summary>
    public string SourceSystem { get; private set; } = null!;

    /// <summary>
    /// 源文件路径（需要转码的源文件路径）
    /// </summary>
    public Uri SourceUrl { get; private set; } = null!;

    /// <summary>
    /// 转码成功的文件路径
    /// </summary>
    public Uri? OutputUrl { get; private set; }

    /// <summary>
    /// 转码格式
    /// </summary>
    public string OutputFormat { get; private set; } = null!;

    /// <summary>
    /// 转码状态
    /// </summary>
    public ItemStatus Status { get; set; }

    /// <summary>
    /// 转码工具输出日志
    /// </summary>
    public string? LogText { get; private set; } = null!;

    /// <summary>
    /// 设置文件的大小以及散列值
    /// </summary>
    /// <param name="fileSizeInBytes"></param>
    /// <param name="fileSHA256Hash"></param>
    public void SetFile(long fileSizeInBytes, string fileSHA256Hash)
    {
        FileSizeInBytes = fileSizeInBytes;
        FileSHA256Hash = fileSHA256Hash;
    }

    /// <summary>
    /// 转码开始
    /// </summary>
    public void Start()
    {
        Status = ItemStatus.Started;
        AddDomainEvent(new EncodingItemStartedEvent(Id, SourceSystem));
        LogText = "转码开始";
    }

    /// <summary>
    /// 转码完成
    /// </summary>
    /// <param name="outputUrl"></param>
    public void Complete(Uri outputUrl)
    {
        Status = ItemStatus.Completed;
        OutputUrl = outputUrl;
        AddDomainEvent(new EncodingItemCompletedEvent(Id, outputUrl, SourceSystem));
        LogText = "转码完成";
    }

    /// <summary>
    /// 转码失败
    /// </summary>
    /// <param name="failMsg"></param>
    public void Fail(string failMsg)
    {
        Status = ItemStatus.Failed;
        AddDomainEvent(new EncodingItemFailedEvent(Id, failMsg, SourceSystem));
        LogText = failMsg;
    }
    public void Fail(Exception exception)
    {
        Fail($"转码失败：{exception.Message}");
    }
}
public enum ItemStatus
{
    Ready,    // 任务刚创建完成
    Started,  // 开始处理
    Completed,// 成功
    Failed,   // 失败
}