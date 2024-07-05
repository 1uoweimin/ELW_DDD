using ListeningService.Domain.Events;
using MediatR;
using Zack.EventBus;

namespace ListeningService.Admin.WebAPI.EventHandlers;

public class EpisodeUpdateEventHandler : INotificationHandler<EpisodeUpdateEvent>
{
    private readonly IEventBus _eventBus;
    public EpisodeUpdateEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(EpisodeUpdateEvent notification, CancellationToken cancellationToken)
    {
        //Todo: 如果音频地址改变，则重新转码。（考虑不能修改音频地址，因为主流视频网站也都是这样做的）

        /* 考虑转码成功后修改AudioUrl：
            实现方式：新增的时候不是真的新增，而是接入队列，因为这时候的Episode如果插入数据库是非法状态，
                      如果允许非法数据插入数据库，就要对逻辑做复杂处理，所以转码之后才能插入数据库。
         */

        var episode = notification.Episode;
        if (episode.IsVisible)
        {
            var eventData = new { episode.Id, episode.Name, episode.AlbumId, episode.Subtitle, Sentences = episode.ParseSubtitle() };
            _eventBus.Publish("ListeningService.Episode.Update", eventData);
        }
        else  
        {
            _eventBus.Publish("ListeningService.Episode.Hide", new { Id = episode.Id });
        }
        return Task.CompletedTask;
    }
}
