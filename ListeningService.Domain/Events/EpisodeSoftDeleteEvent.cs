using MediatR;

namespace ListeningService.Domain.Events;

public record EpisodeSoftDeleteEvent(Guid Id) : INotification;
