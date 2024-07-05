using FluentValidation;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record AlbumAddReq(Guid CategoryId, MultilingualString Name);
public class AlbumAddReqValidator : AbstractValidator<AlbumAddReq>
{
    public AlbumAddReqValidator()
    {
        RuleFor(a => a.Name).NotEmpty();
        RuleFor(a => a.Name.Chinese).NotNull().Length(1, 200);
        RuleFor(a => a.Name.English).NotNull().Length(1, 200);
    }
}