namespace SearchService.DomainService.Entities;

/// <summary>
/// 音频记录
/// </summary>
/// <param name="Id"></param>
/// <param name="CnName"></param>
/// <param name="EngName"></param>
/// <param name="PlainSubtitle"></param>
/// <param name="AlbumId"></param>
public record Episode(Guid Id, string CnName, string EngName, string PlainSubtitle, Guid AlbumId);