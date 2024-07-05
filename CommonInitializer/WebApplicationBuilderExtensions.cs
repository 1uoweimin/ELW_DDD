using CommonInitializer;
using CommonInitializer.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Zack.ASPNETCore;
using Zack.Commons;
using Zack.Commons.JsonConverters;
using Zack.EventBus;
using Zack.JWT;

namespace Microsoft.AspNetCore.Builder;
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// 注册开发环境下的服务
    /// </summary>
    /// <param name="builder"></param>
    public static void AddDevelopment(this WebApplicationBuilder builder, Action<List<ApiVersionInfo.Version>> versionsDescription)
    {
        var services = builder.Services;
        if (builder.Environment.IsDevelopment())
        {
            var avi = new ApiVersionInfo();
            versionsDescription.Invoke(avi.Versions);
            string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;

            // 该服务将扫描应用程序并收集有关路由和端点的信息（通常与Swagger或其他API文档生成工具结合使用）
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                // 设置自定义的 SchemaIdSelector（避免不同命名空间的相同命名的嵌套类型的 schemaId 相同导致冲突）
                // 使用类型的完整名称（包含命名空间）作为 schemaId，确保类型名称的唯一性，并且类型是在不同的命名空间中
                c.CustomSchemaIds(t => t.FullName?.Replace('+', '_'));

                // 版本控制服务注入
                foreach (var version in avi.Versions)
                    c.SwaggerDoc(version.Name, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"Lwm_{assemblyName}",
                        Version = version.Name,
                        Description = version.Description
                    });
            });
        }
    }

    /// <summary>
    /// 初始化服务注册
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="initOptions"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public static void AddInitializer(this WebApplicationBuilder builder, InitializerOptions initOptions)
    {
        IServiceCollection services = builder.Services;
        IConfiguration GetSectionConfiguration(string sectionName) => builder.Configuration.GetSection(sectionName);
        T? GetSectionInstance<T>(string sectionName) => builder.Configuration.GetSection(sectionName).Get<T>();
        string? GetSectionValue(string sectionName) => builder.Configuration.GetSection(sectionName).Value;

        var assemblies = ReflectionHelper.GetAllReferencedAssemblies();

        // 自动注册各个程序集的服务（运行实现了 IModuleInitializer 接口的类的 Initialize 方法）
        //services.RunModuleInitializers(assemblies);

        // 配置中心数据库配置（注意：postgresql数据库中的这个表名小写）
        string? dbConfigConnStr = GetSectionValue(initOptions.DbConfigurationOpt);
        if (string.IsNullOrEmpty(dbConfigConnStr)) throw new ArgumentNullException(dbConfigConnStr);
        builder.Configuration.AddDbConfiguration(() => new NpgsqlConnection(dbConfigConnStr), "T_Configs", true, TimeSpan.FromSeconds(5));

        // 数据库配置
        string dbConnStr = GetSectionValue(initOptions.DbConnStrOpt)!;
        services.AddAllDbContexts(builder => builder.UseNpgsql(dbConnStr), assemblies);

        // MediatR 配置（领域事件）
        services.AddMediatR(assemblies);

        // 跨域配置
        services.AddCors(setupAction =>
        {
            CorsOptions? corsOpt = GetSectionInstance<CorsOptions>(initOptions.CorsOption);
            if (corsOpt == null) throw new NullReferenceException(nameof(corsOpt));

            setupAction.AddDefaultPolicy(configPolicy
                => configPolicy.WithOrigins(corsOpt.Origins).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        });

        // 工作单元筛选器配置
        services.Configure<MvcOptions>(options => options.Filters.Add<UnitOfWorkFilter>());
        // 设置时间格式，而非“2008-08-08T08:08:08”这样的格式
        services.Configure<JsonOptions>(options => options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss")));
        // 所有可用的转发头都应该被处理【X-Forwarded-For（原始请求的IP地址）、X-Forwarded-Proto（原始请求的协议，如http或https）、X-Forwarded-For（原始请求的IP地址）】
        services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);

        // 请求数据校验配置
        if (initOptions.IsFluentValidation)
        {
            services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssemblies(assemblies);
        }

        // JWT 配置（身份验证和配置授权）
        if (initOptions.IsJWT)
        {
            var jwtOptions = GetSectionInstance<JWTOptions>(initOptions.JWTOpt);
            if (jwtOptions == null) throw new NullReferenceException(nameof(jwtOptions));

            services.AddScoped<ITokenService, TokenService>(); // 注册JWT服务
            services.Configure<JWTOptions>(GetSectionConfiguration(initOptions.JWTOpt));
            services.AddJWTAuthentication(jwtOptions);
            // 启用Swagger中的【Authorize】按钮，这样就不用每个项目的 AddSwaggerGen 中单独配置了
            services.Configure<SwaggerGenOptions>(options => options.AddAuthenticationHeader());
        }

        // 日志配置
        if (initOptions.IsLog)
        {
            // 如果读取路径为空，则设置默认路径
            string? logFilePath = GetSectionValue(initOptions.LogFilePathOpt);
            if (string.IsNullOrEmpty(logFilePath))
            {
                string entryAssemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;
                string pullPathDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                logFilePath = Path.Combine(pullPathDir, $"{entryAssemblyName}.log");
            }

            services.AddLogging(config =>
            {
                // postgreSQL 数据库中表的配置
                IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
                {
                    {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                    {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                    {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                    {"timestamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
                    {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                    {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
                    {"props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
                    {"machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text) }
                };

                // 日志输出配置
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    //.WriteTo.Console(new JsonFormatter()) // json 格式
                    .WriteTo.Console()
                    .WriteTo.File(
                        //formatter: new JsonFormatter(), // json 格式
                        path: logFilePath,
                        rollingInterval: RollingInterval.Day, // 滚动的间隔
                        fileSizeLimitBytes: 50 * 1024 * 1024, // 每个文件限制在100m以内
                        retainedFileCountLimit: 31, // 文件夹限制在31个文件之内
                        rollOnFileSizeLimit: true,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1))
                    .WriteTo.PostgreSQL(
                       connectionString: dbConnStr,
                       tableName: "t_systems_logs",
                       columnOptions: columnWriters,
                       needAutoCreateTable: true)
                    .CreateLogger();

                config.AddSerilog();
            });
        }

        // 内存缓存配置
        if (initOptions.IsMemoryCache)
        {
            services.AddMemoryCache();
            services.AddScoped<IMemoryCacheHelper, MemoryCacheHelper>();
        }

        // 分布式缓存配置
        if (initOptions.IsDistributeMemoryCache)
        {
            services.AddDistributedMemoryCache();
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();
        }

        // Redis 配置（缓存服务器）
        if (initOptions.IsRedis)
        {
            var redisOptions = GetSectionInstance<RedisOptions>(initOptions.RedisOpt);
            if (redisOptions == null) throw new NullReferenceException(nameof(redisOptions));

            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisOptions.ConnStr);
            services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);
        }

        // RabbitMQ 配置 （集成事件）
        if (initOptions.IsEventBus)
        {
            string? queueName = initOptions.EventBusQueueName;
            if (string.IsNullOrEmpty(queueName)) throw new ArgumentNullException(queueName);

            services.Configure<IntegrationEventRabbitMQOptions>(GetSectionConfiguration(initOptions.RabbitMQOpt));
            services.AddEventBus(queueName, assemblies);
        }
    }
}