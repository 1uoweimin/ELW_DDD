using MediaEncoderService.Domain.Entities;
using MediatR;

namespace MediaEncoderService.Domain.Events;

public record EncodingItemCreatedEvent(EncodingItem encodingItem) : INotification;
