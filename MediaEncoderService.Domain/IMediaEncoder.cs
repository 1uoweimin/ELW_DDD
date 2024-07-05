namespace MediaEncoderService.Domain;

/// <summary>
/// 转码接口
/// </summary>
public interface IMediaEncoder
{
    /// <summary>
    /// 是否支持处理format类型的文件
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    bool Support(string format);

    /// <summary>
    /// 进行转码
    /// </summary>
    /// <param name="sourceFile">源文件</param>
    /// <param name="destFile">目标文件</param>
    /// <param name="destFormat">目标格式</param>
    /// <param name="arg"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task EncodeAsync(FileInfo sourceFile, FileInfo destFile, string destFormat, string[]? arg, CancellationToken cancellationToken);
}
