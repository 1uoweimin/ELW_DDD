using IdentityService.Domain;
using IdentityService.WebAPI.Events;
using Zack.EventBus;

namespace IdentityService.WebAPI.EventHandlers;

/// <summary>
/// 用户创建处理事件
/// </summary>
[EventName("IdentityService.User.Created")]
public class UserCreateEventHandler : JsonIntegrationEventHandler<UserCreateEvent>
{
    private readonly ISMSSender _smsSender;
    public UserCreateEventHandler(ISMSSender smsSender)
    {
        _smsSender = smsSender;
    }

    public override Task HandleJson(string eventName, UserCreateEvent? eventData)
    {
        return _smsSender.SendAsync(eventData!.phoneNumber, $"新用户，初始化密码：{eventData.password}");
    }
}