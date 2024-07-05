using IdentityService.Domain;
using IdentityService.WebAPI.Events;
using Zack.EventBus;

namespace IdentityService.WebAPI.EventHandlers;

/// <summary>
/// 发送手机验证码处理事件
/// </summary>
[EventName("IdentityService.Login.PhoneVerifiedCode")]
public class SendPhoneVerifiedCodeEventHandler : JsonIntegrationEventHandler<SendPhoneVerifiedCodeEvent>
{
    private readonly ISMSSender _smtpSender;
    public SendPhoneVerifiedCodeEventHandler(ISMSSender smtpSender)
    {
        _smtpSender = smtpSender;
    }

    public override Task HandleJson(string eventName, SendPhoneVerifiedCodeEvent? eventData)
    {
        _smtpSender.SendAsync(eventData!.ToPhoneNumber, $"验证码：{eventData.VerifyCode}");
        return Task.CompletedTask;
    }
}