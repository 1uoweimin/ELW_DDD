using SearchService.DomainService.Entities;

namespace SearchService.WebAPI.Controllers.v2.Responses;
public record SearchEpisodesRsp(IEnumerable<Episode> Episodes, long TotalCount);
