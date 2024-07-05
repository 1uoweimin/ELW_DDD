using IdentityService.Domain;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Email_SMS_Services;

/// <summary>
/// 模拟发送短信服务
/// </summary>
public class MockSMSSender : ISMSSender
{
    private readonly ILogger<MockEmailSender> _logger;
    public MockSMSSender(ILogger<MockEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string phoneNumber, params string[] args)
    {
        _logger.LogWarning($"模拟发送短信：系统给{phoneNumber}手机发送短信，短信内容：【{string.Join('|', args)}】");
        return Task.CompletedTask;
    }
}