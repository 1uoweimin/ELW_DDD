namespace MediaEncoderService.Domain;

/// <summary>
/// 转码工厂
/// </summary>
public class MediaEncoderFactory
{
    private readonly IEnumerable<IMediaEncoder> _encoders;
    public MediaEncoderFactory(IEnumerable<IMediaEncoder> encoders)
    {
        _encoders = encoders;
    }

    /// <summary>
    /// 获得支持format格式的转码实例
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public IMediaEncoder? GetMediaEncoder(string format)
    {
        foreach (var encoder in _encoders)
            if (encoder.Support(format))
                return encoder;

        return null;
    }
}
