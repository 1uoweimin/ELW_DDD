namespace CommonInitializer;

/// <summary>
/// API版本消息类
/// </summary>
public class ApiVersionInfo
{
    public List<Version> Versions { get; set; } = new List<Version>();

    public class Version
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}