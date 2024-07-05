using CommonInitializer.ActionDatas;
using FluentValidation;

namespace SearchService.WebAPI.Controllers.v2.Requests;
public record SearchEpisodesReq(string KeyWord, int PageIndex, int PageSize);

public class SearchEpisodesReqValidator : AbstractValidator<ApiRequest<SearchEpisodesReq>>
{
    public SearchEpisodesReqValidator()
    {
        RuleFor(e => e.ReqData.KeyWord).NotNull().MinimumLength(2).MaximumLength(100);
        RuleFor(e => e.ReqData.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(e => e.ReqData.PageSize).GreaterThanOrEqualTo(5);
    }
}