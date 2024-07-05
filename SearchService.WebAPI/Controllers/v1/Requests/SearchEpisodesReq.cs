using FluentValidation;

namespace SearchService.WebAPI.Controllers.v1.Requests;
public record SearchEpisodesReq(string KeyWord, int PageIndex, int PageSize);

public class SearchEpisodesReqValidator : AbstractValidator<SearchEpisodesReq>
{
    public SearchEpisodesReqValidator()
    {
        RuleFor(e => e.KeyWord).NotNull().MinimumLength(2).MaximumLength(100);
        RuleFor(e => e.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(e => e.PageSize).GreaterThanOrEqualTo(5);
    }
}