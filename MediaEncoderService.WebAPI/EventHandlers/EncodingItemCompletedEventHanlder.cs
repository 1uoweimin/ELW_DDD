using MediaEncoderService.Domain.Events;
using MediatR;
using Zack.EventBus;

namespace MediaEncoderService.WebAPI.EventHandlers;

public class EncodingItemCompletedEventHanlder : INotificationHandler<EncodingItemCompletedEvent>
{
    private readonly IEventBus _eventBus;
    public EncodingItemCompletedEventHanlder(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(EncodingItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        _eventBus.Publish("MediaEncoding.Completed", notification);
        return Task.CompletedTask;
    }
}
