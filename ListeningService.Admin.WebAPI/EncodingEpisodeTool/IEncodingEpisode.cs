using ListeningService.Admin.WebAPI.EncodingEpisodeTool;

namespace ListeningService.Admin.WebAPI.EncodingEpisodeHelper;

/// <summary>
/// Episode转码仓储接口
/// </summary>
public interface IEncodingEpisode
{
    /// <summary>
    /// 增加待转码任务的详细信息
    /// </summary>
    /// <param name="episodeInfo"></param>
    /// <returns></returns>
    Task AddEncodingEpisodeAsync(EncodingEpisodeInfo episodeInfo);

    /// <summary>
    /// 删除一个Episode任务
    /// </summary>
    /// <param name="albumId"></param>
    /// <param name="episodeId"></param>
    /// <returns></returns>
    Task RemoveEncodingEpisodeAsync(Guid albumId, Guid episodeId);

    /// <summary>
    /// 修改Episode的转码状态
    /// </summary>
    /// <param name="db"></param>
    /// <param name="episodeId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task UpdateEncodingEpisodeStatusAsync(Guid episodeId, EEInofStatus status);

    /// <summary>
    /// 获得Episode的转码状态
    /// </summary>
    /// <param name="db"></param>
    /// <param name="episodeId"></param>
    /// <returns></returns>
    Task<EncodingEpisodeInfo> GetEncodingEpisodeAsync(Guid episodeId);

    /// <summary>
    /// 获取这个albumId下所有转码任务
    /// </summary>
    /// <param name="albumId"></param>
    /// <returns></returns>
    Task<IEnumerable<Guid>> GetEncodingEpisodeIdsOfAlbumAsync(Guid albumId);
}
