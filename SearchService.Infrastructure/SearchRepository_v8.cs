using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SearchService.DomainService;
using SearchService.DomainService.Entities;

namespace SearchService.Infrastructure;
public class SearchRepository_v8 : ISearchRepository
{
    private readonly ElasticsearchClient _elasticClient;
    public SearchRepository_v8(ElasticsearchClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task UpsertEpisodeAsync(Episode episode)
    {
        var rep = await _elasticClient.IndexAsync(episode, "episodes", indexDR => indexDR.Id(episode.Id));
        if (!rep.IsValidResponse)
            throw new ApplicationException(rep.DebugInformation);
    }

    public async Task DeleteEpisodeAsync(Guid episodeId)
    {
        var req = await _elasticClient.DeleteAsync(new DeleteRequest("episodes", episodeId));
        if (!req.IsValidResponse)
            throw new ApplicationException(req.DebugInformation);
    }

    public async Task<(IEnumerable<Episode> Episodes, long TotalCount)> SearchEpisodesAsync(string keyWork, int pageIndex, int pageSize)
    {
        string indices = "episodes";
        string cnName = "cnName";
        string engName = "engName";
        string plainSubtitle = "plainSubtitle";

        // 配置搜索数据的条件
        var searchRequest = new SearchRequest<Episode>(indices)
        {
            From = pageSize * (pageIndex - 1),
            Size = pageSize,
            Query = new MultiMatchQuery
            {
                Fields = new[] { cnName, engName, plainSubtitle }, // 搜索的字段 
                Query = keyWork, // 搜索的关键词
            },
            Highlight = new Highlight
            {
                Fields = new Dictionary<Field, HighlightField>
                {
                    { plainSubtitle, new HighlightField{ PreTags = new string[]{ "<em>" } , PostTags = new string[]{ "</em>" } }} // 添加高亮标签 <em></em>
                }
            }
        };

        // 执行搜索请求
        SearchResponse<Episode> searchResponse = await _elasticClient.SearchAsync<Episode>(searchRequest);
        if (!searchResponse.IsValidResponse)
            throw new ApplicationException(searchResponse.DebugInformation);

        // 处理搜索结果 
        List<Episode> episodeList = new List<Episode>();
        foreach (var hit in searchResponse.Hits)
        {
            if (hit.Source == null) continue;

            // 如果没有预览内容，则显示前50个字
            string highLightedSubtitle;
            if (hit.Highlight != null && hit.Highlight.ContainsKey(plainSubtitle))
                highLightedSubtitle = string.Join("\r\n", hit.Highlight[plainSubtitle]);
            else
                highLightedSubtitle = hit.Source.PlainSubtitle.Cut(50);

            var source = hit.Source with { PlainSubtitle = highLightedSubtitle };
            episodeList.Add(source);
        }

        // 返回
        return (episodeList, episodeList.Count);
    }
}
