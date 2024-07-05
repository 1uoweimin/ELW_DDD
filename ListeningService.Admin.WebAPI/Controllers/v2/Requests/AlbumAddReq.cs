using CommonInitializer.ActionDatas;
using FluentValidation;
using Zack.DomainCommons.Models;

namespace ListeningService.Admin.WebAPI.Controllers.v2.Requests;

public record AlbumAddReq(Guid CategoryId, MultilingualString Name);
public class AlbumAddReqValidator : AbstractValidator<ApiRequest<AlbumAddReq>>
{
    public AlbumAddReqValidator()
    {
        RuleFor(a => a.ReqData.Name).NotEmpty();
        RuleFor(a => a.ReqData.Name.Chinese).NotNull().Length(1, 200);
        RuleFor(a => a.ReqData.Name.English).NotNull().Length(1, 200);
    }
}