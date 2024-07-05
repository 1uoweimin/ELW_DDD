using ListeningService.Admin.WebAPI.EncodingEpisodeHelper;
using ListeningService.Admin.WebAPI.EncodingEpisodeTool;

namespace Microsoft.Extensions.DependencyInjection;

public static class EncodingEpisodeExtensions
{
    public static void AddEncodingEpisode(this IServiceCollection services)
    {
        // Todo：还可以编写其他的实现类，来实现IEncodingEpisode接口，例如用数据库来实现存储

        services.AddScoped<IEncodingEpisode, RedisEncodingEpisode>();
    }
}
