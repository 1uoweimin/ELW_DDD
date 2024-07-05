using FluentValidation;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record CategoryUpdateReq(Guid Id, MultilingualString Name, Uri Url);

public class CategoryUpdateReqValidator : AbstractValidator<CategoryAddReq>
{
    public CategoryUpdateReqValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(c => c.Name.English).NotNull().NotEmpty().Length(1, 200);
        RuleFor(c => c.Url).Length(5, 500);
    }
}