using MediaEncoderService.Domain.Events;
using MediatR;
using Zack.EventBus;

namespace MediaEncoderService.WebAPI.EventHandlers;

public class EncodingItemStartedEventHanlder : INotificationHandler<EncodingItemStartedEvent>
{
    private readonly IEventBus _eventBus;
    public EncodingItemStartedEventHanlder(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(EncodingItemStartedEvent notification, CancellationToken cancellationToken)
    {
        _eventBus.Publish("MediaEncoding.Started", notification);
        return Task.CompletedTask;
    }
}
