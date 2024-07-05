namespace CommonInitializer.ActionDatas;

/// <summary>
/// 响应报文头
/// </summary>
/// <typeparam name="T">泛型报文体数据类型</typeparam>
public record ApiResponse<T> where T : class
{
    private ApiResponse(string retCode, string? respMsg, T? respData)
    {
        RespTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();
        RetCode = retCode;
        RespMsg = respMsg;
        RespData = respData;
    }

    /// <summary>
    /// 响应时间
    /// </summary>
    public string RespTime { get; set; }

    /// <summary>
    /// 返回码
    /// </summary>
    public string RetCode { get; set; }

    /// <summary>
    /// 异常时返回错误消息
    /// </summary>
    public string? RespMsg { get; set; }

    /// <summary>
    /// 报文体数据
    /// </summary>
    public T? RespData { get; set; }

    /// <summary>
    /// 响应成功
    /// </summary>
    /// <param name="retCode"></param>
    /// <param name="respData"></param>
    /// <returns></returns>
    public static ApiResponse<T> Succeed(T? respData = default(T))
    {
        string retCode = GetRetCode(RetCodeE.Successful);
        return new ApiResponse<T>(retCode, null, respData);
    }

    /// <summary>
    /// 响应失败
    /// </summary>
    /// <param name="respMsg"></param>
    /// <returns></returns>
    public static ApiResponse<T> Fail(string respMsg)
    {
        return Fail(RetCodeE.Failed, respMsg);
    }
    /// <summary>
    /// 响应失败
    /// </summary>
    /// <param name="retCodeE"></param>
    /// <param name="respMsg"></param>
    /// <returns></returns>
    public static ApiResponse<T> Fail(RetCodeE retCodeE, string respMsg)
    {
        string retCode = GetRetCode(retCodeE);
        return new ApiResponse<T>(retCode, respMsg, null);
    }

    /// <summary>
    /// 响应失败，没找到
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static ApiResponse<T> NotFound(string msg = "Not Found")
    {
        string retCode = GetRetCode(RetCodeE.NotFound);
        return new ApiResponse<T>(retCode, msg, null);
    }

    private static string GetRetCode(RetCodeE retCodeE)
    {
        return ((int)retCodeE).ToString();
    }
}

public enum RetCodeE
{
    Successful = 200,
    Failed = 400,
    NotFound = 404,

    LockedOut = 1001,               // 用户锁定
    IncorrectOfUNameOrPwd = 1002,   // 用户名或密码错误
    IncorrectOfVerification = 1003, // 验证码错误

}