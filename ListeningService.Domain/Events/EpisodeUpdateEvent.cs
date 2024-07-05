using ListeningService.Domain.Entities;
using MediatR;

namespace ListeningService.Domain.Events;

public record EpisodeUpdateEvent(Episode Episode) : INotification;
