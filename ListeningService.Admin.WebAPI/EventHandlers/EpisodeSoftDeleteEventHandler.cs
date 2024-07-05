using ListeningService.Domain.Events;
using MediatR;
using Zack.EventBus;

namespace ListeningService.Admin.WebAPI.EventHandlers;

public class EpisodeSoftDeleteEventHandler : INotificationHandler<EpisodeSoftDeleteEvent>
{
    private readonly IEventBus _eventBus;
    public EpisodeSoftDeleteEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(EpisodeSoftDeleteEvent notification, CancellationToken cancellationToken)
    {
        var id = notification.Id;
        _eventBus.Publish("ListeningService.Episode.SoftDelete", new { Id = id });
        return Task.CompletedTask;
    }
}
