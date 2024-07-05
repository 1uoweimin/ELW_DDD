using IdentityService.Domain;
using IdentityService.WebAPI.Events;
using Zack.EventBus;

namespace IdentityService.WebAPI.EventHandlers;

/// <summary>
/// 发送邮箱验证码处理事件
/// </summary>
[EventName("IdentityService.Login.EmailVerifiedCode")]
public class SendEmailVerifiedCodeEventHandler : JsonIntegrationEventHandler<SendEmailVerifiedCodeEvent>
{
    private readonly IEmailSender _emailSender;
    public SendEmailVerifiedCodeEventHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public override Task HandleJson(string eventName, SendEmailVerifiedCodeEvent? eventData)
    {
        _emailSender.SendAsync(eventData!.ToEmail, "登录验证", $"验证码：{eventData.VerifyCode}");
        return Task.CompletedTask;
    }
}