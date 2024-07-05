using SearchService.DomainService.Entities;

namespace SearchService.DomainService;
public interface ISearchRepository
{
    /// <summary>
    /// 更新或插入音频
    /// </summary>
    /// <param name="episode">音频</param>
    /// <returns></returns>
    Task UpsertEpisodeAsync(Episode episode);

    /// <summary>
    /// 删除音频
    /// </summary>
    /// <param name="episodeId">音频Id</param>
    /// <returns></returns>
    Task DeleteEpisodeAsync(Guid episodeId);

    /// <summary>
    /// 搜索原文存在关键词的音频
    /// </summary>
    /// <param name="keyWork">关键词</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页的条数</param>
    /// <returns></returns>
    Task<(IEnumerable<Episode> Episodes, long TotalCount)> SearchEpisodesAsync(string keyWork, int pageIndex, int pageSize);
}