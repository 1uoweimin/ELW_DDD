using FluentValidation;
using ListeningService.Domain.ValueObjects;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v2.Requests;

public record EpisodeUpdateReq(Guid Id, MultilingualString Name, Subtitle Subtitle);

public class EpisodeUpdateReqValidator : AbstractValidator<EpisodeUpdateReq>
{
    public EpisodeUpdateReqValidator()
    {
        RuleFor(e => e.Name).NotEmpty();
        RuleFor(e => e.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.Name.English).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.Subtitle).NotEmpty();
        RuleFor(e => e.Subtitle.Content).NotNull().NotEmpty();
        RuleFor(e => e.Subtitle.Type).NotNull().NotEmpty().Length(1, 10);
    }
}