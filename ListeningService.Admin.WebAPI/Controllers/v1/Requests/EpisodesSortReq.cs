using CommonInitializer.ActionDatas;
using FluentValidation;
using ListeningService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record EpisodesSortReq(Guid AlbumId, Guid[] Ids);

public class EpisodesSortReqValidator : AbstractValidator<ApiRequest<EpisodesSortReq>>
{
    public EpisodesSortReqValidator(ListeningDbContext dbCtx)
    {
        RuleFor(e => e.ReqData.Ids).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
    }
}