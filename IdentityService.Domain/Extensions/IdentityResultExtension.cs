namespace Microsoft.AspNetCore.Identity;

public static class IdentityResultExtension
{
    /// <summary>
    /// 把 IdentityError集合 转换为字符串
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string ErrorsString(this IdentityResult result)
    {
        IEnumerable<string> errInfos = result.Errors.Select(err => $"[Code:{err.Code}; Description:{err.Description}]");
        return string.Join('|', errInfos);
    }

    /// <summary>
    /// 生成一个包含IdentityError错误消息对象的 IdentityResult 对象
    /// </summary>
    /// <param name="description"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public static IdentityResult ErrorIdentityResult(this string description, string code = "400")
    {
        return IdentityResult.Failed(new IdentityError() { Code = code, Description = description });
    }
}