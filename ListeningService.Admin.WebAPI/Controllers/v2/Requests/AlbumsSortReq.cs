using CommonInitializer.ActionDatas;
using FluentValidation;

namespace ListeningService.Admin.WebAPI.Controllers.v2.Requests;

public record AlbumsSortReq(Guid CategoryId, Guid[] Ids);

public class AlbumsSortReqValidator : AbstractValidator<ApiRequest<AlbumsSortReq>>
{
    public AlbumsSortReqValidator()
    {
        RuleFor(a => a.ReqData.Ids).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
    }
}
