using MediatR;

namespace MediaEncoderService.Domain.Events;

public record EncodingItemStartedEvent(Guid Id, string SourceSystem) : INotification;
