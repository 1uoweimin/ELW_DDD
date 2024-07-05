namespace CommonInitializer.ActionDatas;

/// <summary>
///  请求报文头
/// </summary>
/// <typeparam name="T">泛型报文体数据类型</typeparam>
public record ApiRequest<T> where T : class
{
    /// <summary>
    /// 请求时间（因为是字段，因此不在请求报文头中显示）
    /// </summary>
    public string ReqTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();

    /// <summary>
    /// 报文体数据
    /// </summary>
    public T ReqData { get; set; } = null!;
}