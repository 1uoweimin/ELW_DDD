using CommonInitializer.ActionDatas;
using FluentValidation;
using IdentityService.Domain;

namespace IdentityService.WebAPI.Controllers.v2.Requests;

/// <summary>
/// 添加管理用户请求体
/// </summary>
/// <param name="userName"></param>
/// <param name="phoneNumber"></param>
public record AddUserReq(string UserName, string PhoneNumber, RoleType RoleType);
public class AddAdminUserRequestValidator : AbstractValidator<ApiRequest<AddUserReq>>
{
    public AddAdminUserRequestValidator()
    {
        RuleFor(r => r.ReqData.UserName).NotEmpty().WithMessage("用户名不能为空");
        RuleFor(r => r.ReqData.PhoneNumber).NotEmpty().WithMessage("手机号码不能为空");
    }
}