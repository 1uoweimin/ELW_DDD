using ListeningService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Zack.EventBus;

namespace ListeningService.Admin.WebAPI.EventHandlers
{
    /// <summary>
    /// 监听搜索引擎服务器发送的集成事件消息
    /// </summary>
    [EventName("SearchService.ReIndexAll")]
    public class ReIndexAllEventHandler : IIntegrationEventHandler
    {
        private readonly ListeningDbContext _dbCtx;
        private readonly IEventBus _eventBus;
        public ReIndexAllEventHandler(ListeningDbContext dbCtx, IEventBus eventBus)
        {
            _dbCtx = dbCtx;
            _eventBus = eventBus;
        }

        public Task Handle(string eventName, string eventData)
        {
            ///让搜索引擎服务器，重新收录所有的Episode
            foreach (var episode in _dbCtx.Episode.AsNoTracking())
            {
                if (!episode.IsVisible) continue;
                var episodeEventData = new { episode.Id, episode.Name, episode.AlbumId, episode.Subtitle, Sentences = episode.ParseSubtitle() };
                _eventBus.Publish("ListeningService.Episode.Update", episodeEventData);
            }
            return Task.CompletedTask;
        }
    }
}
