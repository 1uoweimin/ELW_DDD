using FluentValidation;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record AlbumUpdateReq(Guid Id, MultilingualString Name);

public class AlbumUpdateReqValidator : AbstractValidator<AlbumUpdateReq>
{
    public AlbumUpdateReqValidator()
    {
        RuleFor(a => a.Name).NotEmpty();
        RuleFor(a => a.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(a => a.Name.English).NotNull().NotEmpty().Length(1, 200);
    }
}
