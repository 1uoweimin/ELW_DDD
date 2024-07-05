using Zack.DomainCommons.Models;

namespace ListeningService.Domain.Entities;

/// <summary>
/// 类别（充血模型）
/// </summary>
public record Category : AggregateRootEntity, IAggregateRoot
{
    private Category() { } // 供EFCoer使用
    public Category(int sequenceNumber, MultilingualString name, Uri coverUrl)
    {
        SequenceNumber = sequenceNumber;
        Name = name;
        CoverUrl = coverUrl;
    }

    /// <summary>
    /// 序号（越小越靠前）
    /// </summary>
    public int SequenceNumber { get; private set; }

    /// <summary>
    /// 名字（中英两种类型）
    /// </summary>
    public MultilingualString Name { get; private set; } = null!;

    /// <summary>
    /// 封面图片路径
    /// </summary>
    public Uri CoverUrl { get; private set; } = null!;

    public Category ChangeSequenceNumber(int sequenceNumber)
    {
        SequenceNumber = sequenceNumber;
        return this;
    }
    public Category ChangeName(MultilingualString name)
    {
        Name = name;
        return this;
    }
    public Category ChangeCoverUrl(Uri coverUrl)
    {
        CoverUrl = coverUrl;
        return this;
    }
}