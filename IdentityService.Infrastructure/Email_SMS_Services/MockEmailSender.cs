using IdentityService.Domain;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Email_SMS_Services;

/// <summary>
/// 模拟发送邮箱服务
/// </summary>
public class MockEmailSender : IEmailSender
{
    private readonly ILogger<MockEmailSender> _logger;
    public MockEmailSender(ILogger<MockEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string toEmail, string subject, string body)
    {
        _logger.LogWarning($"模拟发送邮箱：系统给{toEmail}邮箱发送主题为“{subject}”的文件，文件内容：【{body}】");
        return Task.CompletedTask;
    }
}