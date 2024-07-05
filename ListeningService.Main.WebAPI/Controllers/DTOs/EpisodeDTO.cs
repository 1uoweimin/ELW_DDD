using ListeningService.Domain.Entities;
using ListeningService.Domain.ValueObjects;
using Zack.DomainCommons.Models;

namespace ListeningService.Main.WebAPI.Controllers.DTOs;

public record EpisodeDTO(Guid Id, MultilingualString Name, Guid AlbumId, Uri AudioUrl, double DurationInSecond, IEnumerable<Sentence>? Sentences)
{
    public static EpisodeDTO? Create(Episode? episode, bool isLoadSubtitle)
    {
        if (episode == null) return null;

        IEnumerable<Sentence>? tempSentences = null;
        if (isLoadSubtitle)
            tempSentences = episode.ParseSubtitle();

        return new EpisodeDTO(episode.Id, episode.Name, episode.AlbumId, episode.AudioUrl, episode.DurationInSecond, tempSentences);
    }
    public static EpisodeDTO[] Create(Episode[] episodes, bool isLoadSubtitle)
        => episodes.Select(e => Create(e, isLoadSubtitle)!).ToArray();
}
