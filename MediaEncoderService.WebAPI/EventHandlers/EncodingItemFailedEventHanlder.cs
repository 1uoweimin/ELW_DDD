using MediaEncoderService.Domain.Events;
using MediatR;
using Zack.EventBus;

namespace MediaEncoderService.WebAPI.EventHandlers;

public class EncodingItemFailedEventHanlder : INotificationHandler<EncodingItemFailedEvent>
{
    private readonly IEventBus _eventBus;
    public EncodingItemFailedEventHanlder(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(EncodingItemFailedEvent notification, CancellationToken cancellationToken)
    {
        _eventBus.Publish("MediaEncoding.Failed", notification);
        return Task.CompletedTask;
    }
}
