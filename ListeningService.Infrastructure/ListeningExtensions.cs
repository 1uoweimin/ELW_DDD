using ListeningService.Domain;
using ListeningService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;
public static class ListeningExtensions
{
    public static void AddListening(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddScoped<ListeningDomainService>();
        services.AddScoped<IListeningRepository, ListeningRepository>();
    }
}
