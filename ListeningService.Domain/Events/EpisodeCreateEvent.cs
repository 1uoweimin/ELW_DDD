using ListeningService.Domain.Entities;
using MediatR;

namespace ListeningService.Domain.Events;

public record EpisodeCreateEvent(Episode Episode) : INotification;
