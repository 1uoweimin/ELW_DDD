using CommonInitializer.ActionDatas;
using FluentValidation;
using ListeningService.Domain.ValueObjects;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record EpisodeUpdateReq(Guid Id, MultilingualString Name, Subtitle Subtitle);

public class EpisodeUpdateReqValidator : AbstractValidator<ApiRequest<EpisodeUpdateReq>>
{
    public EpisodeUpdateReqValidator()
    {
        RuleFor(e => e.ReqData.Name).NotEmpty();
        RuleFor(e => e.ReqData.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.ReqData.Name.English).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.ReqData.Subtitle).NotEmpty();
        RuleFor(e => e.ReqData.Subtitle.Content).NotNull().NotEmpty();
        RuleFor(e => e.ReqData.Subtitle.Type).NotNull().NotEmpty().Length(1, 10);
    }
}