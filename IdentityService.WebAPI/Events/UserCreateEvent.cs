namespace IdentityService.WebAPI.Events;

/// <summary>
/// 用户创建事件
/// </summary>
/// <param name="id"></param>
/// <param name="userName"></param>
/// <param name="password"></param>
/// <param name="phoneNumber"></param>
public record UserCreateEvent(Guid id, string userName, string password, string phoneNumber);
