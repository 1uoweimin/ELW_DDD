using MediatR;

namespace MediaEncoderService.Domain.Events;

public record EncodingItemCompletedEvent(Guid Id, Uri OutputUrl, string SourceSystem) : INotification;