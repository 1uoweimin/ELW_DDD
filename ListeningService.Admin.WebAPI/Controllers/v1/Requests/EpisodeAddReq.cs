using CommonInitializer.ActionDatas;
using FluentValidation;
using ListeningService.Domain.Entities;
using ListeningService.Domain.ValueObjects;
using ListeningService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;
public record EpisodeAddReq(Guid AlbumId, MultilingualString Name, Uri AudioUrl, double DurationInSecond, Subtitle Subtitle);

public class EpisodeAddReqValidator : AbstractValidator<ApiRequest<EpisodeAddReq>>
{
    public EpisodeAddReqValidator(ListeningDbContext dbCtx)
    {
        RuleFor(e => e.ReqData.Name).NotEmpty();
        RuleFor(e => e.ReqData.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.ReqData.Name.English).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.ReqData.AudioUrl).NotEmptyUri().Length(1, 1000);
        RuleFor(e => e.ReqData.DurationInSecond).GreaterThan(0);
        RuleFor(e => e.ReqData.Subtitle).NotEmpty();
        RuleFor(e => e.ReqData.Subtitle.Content).NotNull().NotEmpty();
        RuleFor(e => e.ReqData.Subtitle.Type).NotNull().NotEmpty().Length(1, 10);
    }
}