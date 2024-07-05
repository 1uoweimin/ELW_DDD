using ListeningService.Domain.Events;
using ListeningService.Domain.SubtitleTools;
using ListeningService.Domain.ValueObjects;
using Zack.DomainCommons.Models;

namespace ListeningService.Domain.Entities;

/// <summary>
/// 音频（充血模型）
/// </summary>
public record Episode : AggregateRootEntity, IAggregateRoot
{
    private Episode() { }// 供EFCoer使用
    public Episode(Guid id, int sequenceNumber, MultilingualString name, Guid albumId,
        Uri audioUrl, double durationInSecond, Subtitle subtitle)
    {
        Id = id;
        SequenceNumber = sequenceNumber;
        Name = name;
        IsVisible = false; // 新建以后默认不可见，需要手动设置显示
        AlbumId = albumId;
        AudioUrl = audioUrl;
        DurationInSecond = durationInSecond;
        Subtitle = subtitle;

        AddDomainEvent(new EpisodeCreateEvent(this));
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
    /// 用户是否可见（完善后才显示这个音频）
    /// </summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    /// 和专辑模型的逻辑外键（表示属于哪类别专辑）
    /// </summary>
    public Guid AlbumId { get; private set; }

    /// <summary>
    /// 音频路径
    /// </summary>
    public Uri AudioUrl { get; private set; } = null!;

    /// <summary>
    /// 音频时长（秒）
    /// </summary>
    public double DurationInSecond { get; private set; }

    /// <summary>
    /// 原文字幕（包含内容和类型两个属性）
    /// </summary>
    public Subtitle Subtitle { get; private set; } = null!;

    public override void SoftDelete()
    {
        base.SoftDelete();
        AddDomainEvent(new EpisodeSoftDeleteEvent(Id));
    }
    public Episode ChangeSequenceNumber(int sequenceNumber)
    {
        SequenceNumber = sequenceNumber;
        AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }
    public Episode ChangeName(MultilingualString name)
    {
        Name = name;
        AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }
    public Episode Show()
    {
        IsVisible = true;
        AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }
    public Episode Hide()
    {
        IsVisible = false;
        AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }
    public Episode ChangeAlbumId(Guid albumId)
    {
        AlbumId = albumId;
        AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }
    public Episode ChangeSubtitle(Subtitle subtitle)
    {
        Subtitle = subtitle;
        AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }
    /// <summary>
    /// 把字幕文本Subtitle转换成IEnumerable<Sentence>。
    /// 按照DDD的原则，实体类中不应该考虑对象在数据库中的保存格式，因此应该定义一个Sentence[]类型的属性。
    /// 不过这样设计会导致每次从数据库中读取Episode的时候，都会执行字幕的转换代码，降低程序的性能。
    /// 因此这里做了折中的设计，当程序需要转换字幕原文时，再调用ParseSubtitle方法。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public IEnumerable<Sentence> ParseSubtitle()
    {
        var subtitleParser = SubtitleParserFactory.GetParser(Subtitle.Type);
        if (subtitleParser == null)
            throw new ApplicationException($"It's not support subtitle of ‘{Subtitle.Type}’ type");

        return subtitleParser.Parse(Subtitle.Content);
    }
}