using FluentValidation;
using ListeningService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ListeningService.Admin.WebAPI.Controllers.v2.Requests;

public record EpisodesSortReq(Guid AlbumId, Guid[] Ids);

public class EpisodesSortReqValidator : AbstractValidator<EpisodesSortReq>
{
    public EpisodesSortReqValidator(ListeningDbContext dbCtx)
    {
        RuleFor(e => e.Ids).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
    }
}