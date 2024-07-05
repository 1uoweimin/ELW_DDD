using CommonInitializer;
using CommonInitializer.Options;
using Microsoft.Extensions.Hosting;
using Zack.EventBus;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    /// <summary>
    /// 开发环境中使用的中间件
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseDevelopment(this WebApplication app, Action<List<ApiVersionInfo.Version>> versionsDescription)
    {
        if (app.Environment.IsDevelopment())
        {
            var avi = new ApiVersionInfo();
            versionsDescription.Invoke(avi.Versions);

            app.UseDeveloperExceptionPage(); //用于在开发环境中显示详细的异常信息页面
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // 版本控制中间件
                foreach (var version in avi.Versions)
                    c.SwaggerEndpoint($"/swagger/{version.Name}/swagger.json", version.Name);
            });
        }

        return app;
    }

    /// <summary>
    /// 使用初始化中间件
    /// </summary>
    /// <param name="app"></param>
    /// <param name="initOptions"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseInitializer(this WebApplication app, InitializerOptions initOptions)
    {
        // 注意：因为 UseHttpsRedirection 不能与 ForwardedHeaders 很好的工作，而且 webapi 项目也没必要配置 UseHttpsRedirection 中间件。

        if (initOptions.IsEventBus)
        {
            app.UseEventBus();
        }

        app.UseCors();
        app.UseForwardedHeaders();

        if (initOptions.IsJWT)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        return app;
    }
}