namespace IdentityService.WebAPI.Events;

/// <summary>
/// 重置密码事件
/// </summary>
/// <param name="Id"></param>
/// <param name="UserName"></param>
/// <param name="Password"></param>
/// <param name="PhoneNumber"></param>
public record ResetPasswordEvent(Guid Id, string UserName, string Password, string PhoneNumber);
