namespace CommonInitializer.Options;
public class InitializerOptions
{
    public InitializerOptions(string logFilePathOption, string? eventBusQueueName, bool isFluentValidation = true,
        bool isJwt = true, bool isLog = true, bool isMemoryCache = true, bool isDistributeMemoryCache = true, bool isRedis = true)
    {
        DbConfigurationOpt = "PostgreSqlDB:ConnStr";
        DbConnStrOpt = "PostgreSqlDB:ConnStr";
        CorsOption = "Cors";

        IsFluentValidation = isFluentValidation;

        IsJWT = isJwt;
        JWTOpt = "JWT";

        IsLog = isLog;
        LogFilePathOpt = logFilePathOption;

        IsMemoryCache = isMemoryCache;
        IsDistributeMemoryCache = isDistributeMemoryCache;

        IsRedis = isRedis;
        RedisOpt = "Redis";

        if (string.IsNullOrEmpty(eventBusQueueName)) IsEventBus = false;
        else IsEventBus = true;
        RabbitMQOpt = "RabbitMQ";
        EventBusQueueName = eventBusQueueName;
    }

    /// <summary>
    /// Name：数据库配置中心服务器的连接字符串配置（必须不能为空的）
    /// </summary>
    public string DbConfigurationOpt { get; init; }

    /// <summary>
    /// 数据库连接字符串配置
    /// </summary>
    public string DbConnStrOpt { get; init; }

    /// <summary>
    /// 跨域配置
    /// </summary>
    public string CorsOption { get; init; }

    /// <summary>
    /// 是否注册请求数据校验服务
    /// </summary>
    public bool IsFluentValidation { get; init; }

    /// <summary>
    /// 是否注册JWT服务
    /// </summary>
    public bool IsJWT { get; init; }
    /// <summary>
    /// JWT 的配置
    /// </summary>
    public string JWTOpt { get; init; }

    /// <summary>
    /// 是否启动日志服务
    /// </summary>
    public bool IsLog;
    /// <summary>
    /// 日志输出路径配置
    /// </summary>
    public string LogFilePathOpt { get; init; }

    /// <summary>
    /// 是否注册内存缓存服务
    /// </summary>
    public bool IsMemoryCache { get; init; }

    /// <summary>
    /// 是否注册分布式缓存服务
    /// </summary>
    public bool IsDistributeMemoryCache { get; init; }

    /// <summary>
    /// 是否注册Redis服务
    /// </summary>
    public bool IsRedis { get; init; }
    /// <summary>
    /// Redis 的连接字符串配置
    /// </summary>
    public string RedisOpt { get; init; }

    public bool IsEventBus { get; init; }
    /// <summary>
    /// RabbitMQ 的配置
    /// </summary>
    public string RabbitMQOpt;
    /// <summary>
    /// EventBus的QueueName（如果是NullOrEmpty那么IsRabbitMQ=false,反之IsRabbitMQ=true）
    /// </summary>
    public string? EventBusQueueName { get; init; }
}