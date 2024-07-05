namespace ListeningService.Domain.SubtitleTools;

/// <summary>
/// 原文字幕解析器工厂
/// </summary>
public static class SubtitleParserFactory
{
    private static List<ISubtitlesParser> subtitleParsers = new List<ISubtitlesParser>();
    static SubtitleParserFactory()
    {
        //扫描本程序集中的所有实现了ISubtitleParser接口的类
        var parserTypes = typeof(SubtitleParserFactory).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(ISubtitlesParser).IsAssignableFrom(t));

        //创建这些对象，添加到subtitleParsers
        foreach (var parserType in parserTypes)
        {
            var parser = (ISubtitlesParser)Activator.CreateInstance(parserType)!;
            subtitleParsers.Add(parser);
        }
    }

    /// <summary>
    /// 获取解析subtitleType类型的解析器
    /// </summary>
    /// <param name="subtitleType"></param>
    /// <returns>返回支持subtitleType类型的解析器，如果没有则返回为空</returns>
    public static ISubtitlesParser? GetParser(string subtitleType)
    {
        //遍历所有解析器，把能解析subtitleType类型的解析器返回
        foreach (var subtitleParser in subtitleParsers)
        {
            if (subtitleParser.Support(subtitleType))
                return subtitleParser;
        }
        return null;
    }
}