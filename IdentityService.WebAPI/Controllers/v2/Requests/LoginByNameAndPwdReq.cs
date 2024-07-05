using CommonInitializer.ActionDatas;
using FluentValidation;

namespace IdentityService.WebAPI.Controllers.v2.Requests;

/// <summary>
/// 通过用户名和密码登录请求体
/// </summary>
/// <param name="userName"></param>
/// <param name="password"></param>
public record LoginByNameAndPwdReq(string UserName, string Password);
public class LoginByNameAndPwdRequestValidator : AbstractValidator<ApiRequest<LoginByNameAndPwdReq>>
{
    public LoginByNameAndPwdRequestValidator()
    {
        RuleFor(r => r.ReqData.UserName)
            .NotEmpty().WithMessage("用户名不能为空");
        RuleFor(r => r.ReqData.Password)
            .NotEmpty().WithMessage("密码不能为空");
    }
}
