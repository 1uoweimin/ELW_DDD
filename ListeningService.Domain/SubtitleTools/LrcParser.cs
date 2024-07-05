using ListeningService.Domain.ValueObjects;
using Opportunity.LrcParser;

namespace ListeningService.Domain.SubtitleTools;

/// <summary>
/// *.Lrc 类型的解析器
/// </summary>
public class LrcParser : ISubtitlesParser
{
    public bool Support(string subtitleType)
        => subtitleType.Equals("lrc", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<Sentence> Parse(string subtitle)
    {
        IParseResult<Line> result = Lyrics.Parse(subtitle);
        if (result.Exceptions.Count > 0)
            throw new ApplicationException("Lrc parse fail");
        result.Lyrics.PreApplyOffset();//应用上[offset:500]这样的偏移

        LineCollection<Line> lines = result.Lyrics.Lines;
        Sentence[] sentences = new Sentence[lines.Count];
        for (int i = 0; i < lines.Count - 1; i++)
        {
            if (i < lines.Count - 1)
            {
                sentences[i] = new Sentence(
                StartTime: lines[i].Timestamp.TimeOfDay,
                EndTime: lines[i + 1].Timestamp.TimeOfDay,
                Content: lines[i].Content);
            }
            else
            {
                sentences[i] = new Sentence(
                    StartTime: lines[i].Timestamp.TimeOfDay,
                    // lrc没有结束时间，就极端假定最后一句耗时1分钟
                    EndTime: lines[i].Timestamp.TimeOfDay.Add(TimeSpan.FromMinutes(1)),
                    Content: lines[i].Content);
            }
        }
        return sentences;
    }
}
