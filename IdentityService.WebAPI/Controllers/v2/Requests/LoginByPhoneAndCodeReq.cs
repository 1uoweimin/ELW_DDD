using CommonInitializer.ActionDatas;
using FluentValidation;

namespace IdentityService.WebAPI.Controllers.v2.Requests;

/// <summary>
/// 通过手机号和验证码登录请求体
/// </summary>
/// <param name="phoneNumber"></param>
/// <param name="verifiedCode"></param>
public record LoginByPhoneAndCodeReq(string PhoneNumber, string VerifiedCode);
public class LoginByPhoneAndCodeRequestValidator : AbstractValidator<ApiRequest<LoginByPhoneAndCodeReq>>
{
    public LoginByPhoneAndCodeRequestValidator()
    {
        RuleFor(r => r.ReqData.PhoneNumber)
            .NotEmpty().WithMessage("手机号不能为空");
        RuleFor(r => r.ReqData.VerifiedCode)
            .NotEmpty().WithMessage("验证码不能为空");
    }
}