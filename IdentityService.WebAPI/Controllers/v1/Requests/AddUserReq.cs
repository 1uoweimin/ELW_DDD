using FluentValidation;
using IdentityService.Domain;

namespace IdentityService.WebAPI.Controllers.v1.Requests;

/// <summary>
/// 添加管理用户请求体
/// </summary>
/// <param name="UserName"></param>
/// <param name="PhoneNumber"></param>
public record AddUserReq(string UserName, string PhoneNumber, RoleType RoleType);
public class AddAdminUserRequestValidator : AbstractValidator<AddUserReq>
{
    public AddAdminUserRequestValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().WithMessage("用户名不能为空");
        RuleFor(r => r.PhoneNumber).NotEmpty().WithMessage("手机号码不能为空");
    }
}