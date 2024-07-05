using FluentValidation;
using ListeningService.Domain.Entities;
using ListeningService.Domain.ValueObjects;
using ListeningService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v2.Requests;
public record EpisodeAddReq(Guid AlbumId, MultilingualString Name, Uri AudioUrl, double DurationInSecond, Subtitle Subtitle);

public class EpisodeAddReqValidator : AbstractValidator<EpisodeAddReq>
{
    public EpisodeAddReqValidator(ListeningDbContext dbCtx)
    {
        RuleFor(e => e.Name).NotEmpty();
        RuleFor(e => e.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.Name.English).NotNull().NotEmpty().Length(1, 200);
        RuleFor(e => e.AudioUrl).NotEmptyUri().Length(1, 1000);
        RuleFor(e => e.DurationInSecond).GreaterThan(0);
        RuleFor(e => e.Subtitle).NotEmpty();
        RuleFor(e => e.Subtitle.Content).NotNull().NotEmpty();
        RuleFor(e => e.Subtitle.Type).NotNull().NotEmpty().Length(1, 10);
    }
}