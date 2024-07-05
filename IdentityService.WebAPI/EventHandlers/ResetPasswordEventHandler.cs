using IdentityService.Domain;
using IdentityService.WebAPI.Events;
using Zack.EventBus;

namespace IdentityService.WebAPI.EventHandlers;

/// <summary>
/// 重置密码事件处理
/// </summary>
[EventName("IdentityService.User.PasswordReset")]
public class ResetPasswordEventHandler : JsonIntegrationEventHandler<ResetPasswordEvent>
{
    private readonly ISMSSender _smsSender;
    public ResetPasswordEventHandler(ISMSSender smsSender)
    {
        _smsSender = smsSender;
    }

    public override Task HandleJson(string eventName, ResetPasswordEvent? eventData)
    {
        //发送密码给用户的手机
        return _smsSender.SendAsync(eventData!.PhoneNumber, $"重新设置密码：{eventData.Password}");
    }
}