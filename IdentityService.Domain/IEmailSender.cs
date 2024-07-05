namespace IdentityService.Domain;

/// <summary>
/// 邮箱防腐层
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// 发送邮箱
    /// </summary>
    /// <param name="toEmail">目标邮箱</param>
    /// <param name="subject">主题</param>
    /// <param name="body">内容</param>
    /// <returns></returns>
    public Task SendAsync(string toEmail, string subject, string body);
}