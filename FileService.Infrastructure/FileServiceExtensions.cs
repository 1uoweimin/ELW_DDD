using CommonInitializer.Options;
using FileService.Domain;
using FileService.Infrastructure;
using FileService.Infrastructure.FSStorages;
using FileService.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;
public static class FileServiceExtensions
{
    public static void AddFileService(this WebApplicationBuilder builder, string smbFSStorageOpt, string fileServiceEndpointOpt)
    {
        var services = builder.Services;
        services.AddHttpContextAccessor(); //注册 IHttpContextAccessor 服务的实例
        services.Configure<SMBFSStorageOptions>(builder.Configuration.GetSection(smbFSStorageOpt));
        services.Configure<FileServiceOptions>(builder.Configuration.GetSection(fileServiceEndpointOpt));
        services.AddScoped<FSDomainService>();
        services.AddScoped<IFSRepository, FSRepository>();
        services.AddScoped<IFSStorage, SMBFSStorage>();
        services.AddScoped<IFSStorage, MockFSCloudStorage>();
    }
}
