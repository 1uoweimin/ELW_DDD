using MediatR;

namespace MediaEncoderService.Domain.Events;

public record EncodingItemFailedEvent(Guid Id, string failMsg, string SourceSystem) : INotification;
