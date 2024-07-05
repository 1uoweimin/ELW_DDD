using SearchService.DomainService.Entities;

namespace SearchService.WebAPI.Controllers.v1.Responses;
public record SearchEpisodesRsp(IEnumerable<Episode> Episodes, long TotalCount);
