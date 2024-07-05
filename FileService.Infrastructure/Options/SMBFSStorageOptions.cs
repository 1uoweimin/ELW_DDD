namespace FileService.Infrastructure.Options;

public class SMBFSStorageOptions
{
    // 配置属性不能写成 private set，会导致不注入。
    public string WorkingDir { get; set; } = null!;
}
