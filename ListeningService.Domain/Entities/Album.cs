using Zack.DomainCommons.Models;

namespace ListeningService.Domain.Entities;

/// <summary>
/// 专辑（充血模型）
/// </summary>
public record Album : AggregateRootEntity, IAggregateRoot
{
    private Album() { }// 供EFCoer使用
    public Album(int sequenceNumber, MultilingualString name, Guid categoryId)
    {
        SequenceNumber = sequenceNumber;
        Name = name;
        IsVisible = false; // 新建以后默认不可见，需要手动设置显示
        CategoryId = categoryId;
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
    /// 用户是否可见（完善后才显示这个专辑）
    /// </summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    /// 和类别模型的逻辑外键（表示属于哪类别模型）
    /// </summary>
    public Guid CategoryId { get; private set; }

    public Album ChangeSequenceNumber(int sequenceNumber)
    {
        SequenceNumber = sequenceNumber;
        return this;
    }
    public Album ChangeName(MultilingualString name)
    {
        Name = name;
        return this;
    }
    public Album Show()
    {
        IsVisible = true;
        return this;
    }
    public Album Hide()
    {
        IsVisible = false;
        return this;
    }
    public Album ChangeCategoryId(Guid categoryId)
    {
        CategoryId = categoryId;
        return this;
    }
}