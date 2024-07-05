using ListeningService.Admin.WebAPI.EncodingEpisodeHelper;
using StackExchange.Redis;

namespace ListeningService.Admin.WebAPI.EncodingEpisodeTool
{
    /// <summary>
    /// Redis的Episode转码仓储实现
    /// </summary>
    public class RedisEncodingEpisode : IEncodingEpisode
    {
        private readonly IConnectionMultiplexer _redisConn;
        public RedisEncodingEpisode(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisConn = connectionMultiplexer;
        }

        public string GetStatusKeyForEncodingEpisode(Guid episodeId) => $"ListeningService.EncodingEpisode.{episodeId}";
        public string GetKeyForEncodingEpisodeIdsOfAlbum(Guid albumId) => $"ListeningService.EncodingEpisodeIdsOfAlbum.{albumId}";

        public async Task AddEncodingEpisodeAsync(EncodingEpisodeInfo episodeInfo)
        {
            string statusKeyForEpisode = GetStatusKeyForEncodingEpisode(episodeInfo.Id);
            string keyForEncodingEpisodeIdsOfAlbum = GetKeyForEncodingEpisodeIdsOfAlbum(episodeInfo.AlbumId);

            var db = _redisConn.GetDatabase();
            await db.StringSetAsync(statusKeyForEpisode, episodeInfo.ToJsonString());
            await db.SetAddAsync(keyForEncodingEpisodeIdsOfAlbum, episodeInfo.Id.ToString());
        }
        public async Task RemoveEncodingEpisodeAsync(Guid albumId, Guid episodeId)
        {
            string statusKeyForEpisode = GetStatusKeyForEncodingEpisode(episodeId);
            string keyForEncodingEpisodeIdsOfAlbum = GetKeyForEncodingEpisodeIdsOfAlbum(albumId);

            var db = _redisConn.GetDatabase();
            await db.KeyDeleteAsync(statusKeyForEpisode);
            await db.SetRemoveAsync(keyForEncodingEpisodeIdsOfAlbum, episodeId.ToString());
        }
        public async Task UpdateEncodingEpisodeStatusAsync(Guid episodeId, EEInofStatus status)
        {
            string statusKeyForEpisode = GetStatusKeyForEncodingEpisode(episodeId);

            var db = _redisConn.GetDatabase();
            string json = await db.StringGetAsync(statusKeyForEpisode);
            EncodingEpisodeInfo episodeInfo = json.ParseJson<EncodingEpisodeInfo>()!;
            episodeInfo = episodeInfo with { Status = status };
            await db.StringSetAsync(statusKeyForEpisode, episodeInfo.ToJsonString());
        }
        public async Task<EncodingEpisodeInfo> GetEncodingEpisodeAsync(Guid episodeId)
        {
            string statusKeyForEpisode = GetStatusKeyForEncodingEpisode(episodeId);

            var db = _redisConn.GetDatabase();
            string json = await db.StringGetAsync(statusKeyForEpisode);
            EncodingEpisodeInfo episodeInfo = json.ParseJson<EncodingEpisodeInfo>()!;
            return episodeInfo;
        }
        public async Task<IEnumerable<Guid>> GetEncodingEpisodeIdsOfAlbumAsync(Guid albumId)
        {
            string keyForEncodingEpisodeIdsOfAlbum = GetKeyForEncodingEpisodeIdsOfAlbum(albumId);

            var db = _redisConn.GetDatabase();
            var values = await db.SetMembersAsync(keyForEncodingEpisodeIdsOfAlbum);
            return values.Select(v => Guid.Parse(v));
        }
    }
}
