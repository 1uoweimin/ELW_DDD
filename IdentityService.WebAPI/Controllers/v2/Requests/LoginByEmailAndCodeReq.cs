using CommonInitializer.ActionDatas;
using FluentValidation;

namespace IdentityService.WebAPI.Controllers.v2.Requests;

/// <summary>
/// 通过邮箱和验证码登录请全体
/// </summary>
/// <param name="email"></param>
/// <param name="verifiedCode"></param>
public record LoginByEmailAndCodeReq(string Email, string VerifiedCode);
public class LoginByEmailAndCodeRequestValidator : AbstractValidator<ApiRequest<LoginByEmailAndCodeReq>>
{
    public LoginByEmailAndCodeRequestValidator()
    {
        RuleFor(r => r.ReqData.Email)
           .NotEmpty().WithMessage("邮箱地址能为空")
           .EmailAddress().WithMessage("请输入有效的邮箱地址");
        RuleFor(r => r.ReqData.VerifiedCode)
            .NotEmpty().WithMessage("验证码不能为空");
    }
}
