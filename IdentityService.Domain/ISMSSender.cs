namespace IdentityService.Domain;

/// <summary>
/// 短信发送防腐层
/// </summary>
public interface ISMSSender
{
    /// <summary>
    /// 发送短信
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <param name="args">短信内容</param>
    /// <returns></returns>
    public Task SendAsync(string phoneNumber, params string[] args);
}