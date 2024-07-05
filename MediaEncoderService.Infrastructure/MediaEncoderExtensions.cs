using CommonInitializer.Options;
using MediaEncoderService.Domain;
using MediaEncoderService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

public static class MediaEncoderExtensions
{
    public static void AddMediaEncoder(this WebApplicationBuilder builder, string fsEndpointOpt)
    {
        IServiceCollection services = builder.Services;
        services.Configure<FileServiceOptions>(builder.Configuration.GetSection(fsEndpointOpt));
        services.AddScoped<IMediaEncoderRepository, MediaEncoderRepositroy>();
        services.AddScoped<IMediaEncoder, M4AEncoder>();
        services.AddScoped<MediaEncoderFactory>();
    }
}

