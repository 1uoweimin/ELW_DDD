using CommonInitializer.ActionDatas;
using FluentValidation;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v2.Requests;

public record CategoryUpdateReq(Guid Id, MultilingualString Name, Uri Url);

public class CategoryUpdateReqValidator : AbstractValidator<ApiRequest<CategoryUpdateReq>>
{
    public CategoryUpdateReqValidator()
    {
        RuleFor(c => c.ReqData.Name).NotEmpty();
        RuleFor(c => c.ReqData.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(c => c.ReqData.Name.English).NotNull().NotEmpty().Length(1, 200);
        RuleFor(c => c.ReqData.Url).Length(5, 500);
    }
}