using Nest;
using SearchService.DomainService;
using SearchService.DomainService.Entities;

namespace SearchService.Infrastructure;
public class SearchRepository_v7 : ISearchRepository
{
    private readonly IElasticClient _elasticClient;
    public SearchRepository_v7(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task UpsertEpisodeAsync(Episode episode)
    {
        var IndexRep = await _elasticClient.IndexAsync(episode, IndexD => IndexD.Index("episodes").Id(episode.Id));
        if (!IndexRep.IsValid)
            throw IndexRep.OriginalException;
    }

    public Task DeleteEpisodeAsync(Guid episodeId)
    {
        _elasticClient.DeleteByQuery<Episode>(d => d
            .Index("episodes")
            .Query(q => q.Term(t => t.Id, "elasticsearch.pm")));
        //因为有可能文档不存在，所以不检查结果
        //如果Episode被删除，则把对应的数据也从Elastic Search中删除
        return _elasticClient.DeleteAsync(new DeleteRequest("episodes", episodeId));
    }

    public async Task<(IEnumerable<Episode> Episodes, long TotalCount)> SearchEpisodesAsync(string keyWork, int pageIndex, int pageSize)
    {
        int from = pageSize * (pageIndex - 1);
        string kw = keyWork;

        Func<QueryContainerDescriptor<Episode>, QueryContainer> query = q
            => q.Match(mq => mq.Field(f => f.CnName).Query(kw))
            || q.Match(mq => mq.Field(f => f.EngName).Query(kw))
            || q.Match(mq => mq.Field(f => f.PlainSubtitle).Query(kw));
        Func<HighlightDescriptor<Episode>, IHighlight> highlight = h
            => h.Fields(fs => fs.Field(f => f.PlainSubtitle));
        ISearchResponse<Episode> result = await _elasticClient.SearchAsync<Episode>(s
            => s.Index("episodes").From(from).Size(pageSize).Query(query).Highlight(highlight));

        if (!result.IsValid)
            throw result.OriginalException;

        List<Episode> episodes = new List<Episode>();
        foreach (var hit in result.Hits)
        {
            string highLightedSubtitle;
            // 如果没有预览内容，则显示前50个字
            if (hit.Highlight.ContainsKey("plainSubtitle"))
                highLightedSubtitle = string.Join("\r\n", hit.Highlight["plainSubtitle"]);
            else
                highLightedSubtitle = hit.Source.PlainSubtitle.Cut(50);

            var episode = hit.Source with { PlainSubtitle = highLightedSubtitle };
            episodes.Add(episode);
        }
        return (episodes, result.Total);
    }
}