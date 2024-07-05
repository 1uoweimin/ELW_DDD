using FluentValidation;

namespace ListeningService.Admin.WebAPI.Controllers.v1.Requests;

public record AlbumsSortReq(Guid CategoryId, Guid[] Ids);

public class AlbumsSortReqValidator : AbstractValidator<AlbumsSortReq>
{
    public AlbumsSortReqValidator()
    {
        RuleFor(a => a.Ids).NotNull().NotEmpty().NotContains(Guid.Empty).NotDuplicated();
    }
}
