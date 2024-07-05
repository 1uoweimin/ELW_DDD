using IdentityService.Domain.Entities;

namespace IdentityService.WebAPI.Controllers;

/// <summary>
/// 用户数据转换对象（不要机密信息传递到客户端）
/// </summary>
/// <param name="Id"></param>
/// <param name="UserName"></param>
/// <param name="PhoneNumber"></param>
/// <param name="Email"></param>
/// <param name="CreateTime"></param>
public record UserDTO(Guid Id, string UserName, string? PhoneNumber, string? Email, DateTime CreateTime)
{
    public static UserDTO Create(User user)
        => new UserDTO(user.Id, user.UserName!, user.PhoneNumber, user.Email, user.CreationTime);
}
