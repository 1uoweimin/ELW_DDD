using CommonInitializer.ActionDatas;
using FluentValidation;

namespace IdentityService.WebAPI.Controllers.v2.Requests;

/// <summary>
/// 重设密码请求体
/// </summary>
/// <param name="password"></param>
/// <param name="password2"></param>
public record RestPasswordReq(string Password, string Password2);
public class RestPasswordRequestValidator : AbstractValidator<ApiRequest<RestPasswordReq>>
{
    public RestPasswordRequestValidator()
    {
        RuleFor(r => r.ReqData.Password)
            .NotEmpty().WithMessage("用户名不能为空");
        RuleFor(r => r.ReqData.Password2)
            .NotEmpty().WithMessage("密码不能为空")
            .Equal(r => r.ReqData.Password).WithMessage("两个密码必须一致");
    }
}
