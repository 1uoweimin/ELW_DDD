using ListeningService.Domain.ValueObjects;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.EncodingEpisodeTool;

public enum EEInofStatus { Created = 0, Started, Completed, Failed }
public record EncodingEpisodeInfo(Guid Id, MultilingualString Name, Guid AlbumId, double DurationInSecond, Subtitle Subtitle, EEInofStatus Status);
