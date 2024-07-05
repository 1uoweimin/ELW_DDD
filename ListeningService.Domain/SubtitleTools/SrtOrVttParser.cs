using ListeningService.Domain.ValueObjects;
using SubtitlesParser.Classes.Parsers;
using System.Text;

namespace ListeningService.Domain.SubtitleTools;

/// <summary>
/// *.Srt、*.Wtt 等类型的解析器
/// </summary>
public class SrtOrVttParser : ISubtitlesParser
{
    public bool Support(string subtitleType)
        => subtitleType.Equals("srt", StringComparison.OrdinalIgnoreCase)
        || subtitleType.Equals("vtt", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<Sentence> Parse(string subtitle)
    {
        var parser = new SubParser();
        using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(subtitle));
        var subtitleItems = parser.ParseStream(stream);
        return subtitleItems.Select(s => new Sentence(
            StartTime: TimeSpan.FromMicroseconds(s.StartTime),
            EndTime: TimeSpan.FromMilliseconds(s.EndTime),
            Content: string.Join(" ", s.Lines)));
    }
}
