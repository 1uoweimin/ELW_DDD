using MediaEncoderService.Domain.Events;
using MediatR;
using Zack.EventBus;

namespace MediaEncoderService.WebAPI.EventHandlers;

public class EncodingItemCreatedEventHanlder : INotificationHandler<EncodingItemCreatedEvent>
{
    private readonly IEventBus _eventBus;
    public EncodingItemCreatedEventHanlder(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(EncodingItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
