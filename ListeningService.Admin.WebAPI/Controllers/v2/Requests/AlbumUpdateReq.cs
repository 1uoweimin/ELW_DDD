using CommonInitializer.ActionDatas;
using FluentValidation;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v2.Requests;

public record AlbumUpdateReq(Guid Id, MultilingualString Name);

public class AlbumUpdateReqValidator : AbstractValidator<ApiRequest<AlbumUpdateReq>>
{
    public AlbumUpdateReqValidator()
    {
        RuleFor(a => a.ReqData.Name).NotEmpty();
        RuleFor(a => a.ReqData.Name.Chinese).NotNull().NotEmpty().Length(1, 200);
        RuleFor(a => a.ReqData.Name.English).NotNull().NotEmpty().Length(1, 200);
    }
}
