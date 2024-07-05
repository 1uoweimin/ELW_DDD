using FluentValidation;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record CategoriesSortReq(Guid[] Ids);

public class CategorySortReqValidator : AbstractValidator<CategoriesSortReq>
{
    public CategorySortReqValidator()
    {
        RuleFor(c => c.Ids).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
    }
}