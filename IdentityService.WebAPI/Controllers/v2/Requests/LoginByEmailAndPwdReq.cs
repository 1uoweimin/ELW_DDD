using CommonInitializer.ActionDatas;
using FluentValidation;

namespace IdentityService.WebAPI.Controllers.v2.Requests;

/// <summary>
/// 通过邮箱和密码登录请求体
/// </summary>
/// <param name="email"></param>
/// <param name="password"></param>
public record LoginByEmailAndPwdReq(string Email, string Password);
public class LoginByEmailAndPwdRequestValidator : AbstractValidator<ApiRequest<LoginByEmailAndPwdReq>>
{
    public LoginByEmailAndPwdRequestValidator()
    {
        RuleFor(r => r.ReqData.Email)
            .NotEmpty().WithMessage("邮箱地址能为空")
            .EmailAddress().WithMessage("请输入有效的邮箱地址");
        RuleFor(r => r.ReqData.Password)
            .NotEmpty().WithMessage("密码不能为空");
    }
}
