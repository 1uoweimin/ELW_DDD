namespace ListeningService.Domain.ValueObjects;

public record Sentence(TimeSpan StartTime, TimeSpan EndTime, string Content);
