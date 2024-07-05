using CommonInitializer.ActionDatas;
using FluentValidation;

namespace FileService.WebAPI.Controllers.v2.Requests;
public record FileExistReq(long FileSizeInBytes, string FileSHA256Hash);
public class FileExistValidator : AbstractValidator<ApiRequest<FileExistReq>>
{
    public FileExistValidator()
    {
        RuleFor(f => f.ReqData.FileSizeInBytes).Must(f => f > 0);
        RuleFor(f => f.ReqData.FileSHA256Hash).NotNull().NotEmpty();
    }
}