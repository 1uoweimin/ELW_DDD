using FluentValidation;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record CategoryAddReq(MultilingualString Name, Uri Url);
public class CategoryAddReqValidator : AbstractValidator<CategoryAddReq>
{
    public CategoryAddReqValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(c => c.Name.English).NotNull().NotEmpty().Length(1, 200);
        RuleFor(c => c.Url).Length(5, 500); // CoverUrl允许为空
    }
}