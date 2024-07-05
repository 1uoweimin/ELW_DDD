using ListeningService.Domain.ValueObjects;

namespace ListeningService.Domain.SubtitleTools;

/// <summary>
/// 原文字幕解析器接口
/// </summary>
public interface ISubtitlesParser
{
    /// <summary>
    /// 是否支持subtitleType类型的原文字幕
    /// </summary>
    /// <param name="subtitleType"></param>
    /// <returns></returns>
    bool Support(string subtitleType);

    /// <summary>
    /// 解析原文字幕subtitle
    /// </summary>
    /// <param name="subtitle"></param>
    /// <returns></returns>
    IEnumerable<Sentence> Parse(string subtitle);
}
