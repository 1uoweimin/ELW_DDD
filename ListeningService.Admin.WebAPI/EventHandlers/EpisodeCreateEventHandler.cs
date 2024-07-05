using ListeningService.Domain.Events;
using MediatR;
using Zack.EventBus;

namespace ListeningService.Admin.WebAPI.Events;

public class EpisodeCreateEventHandler : INotificationHandler<EpisodeCreateEvent>
{
    private IEventBus _eventBus;
    public EpisodeCreateEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(EpisodeCreateEvent notification, CancellationToken cancellationToken)
    {
        //把领域事件转发为集成事件，让其他微服务听到

        var episode = notification.Episode;
        //发布集成事件，实现搜索索引、记录日志等功能
        var eventData = new { episode.Id, episode.Name, episode.AlbumId, episode.Subtitle, Sentences = episode.ParseSubtitle() };
        _eventBus.Publish("ListeningService.Episode.Create", eventData);
        return Task.CompletedTask;
    }
}
