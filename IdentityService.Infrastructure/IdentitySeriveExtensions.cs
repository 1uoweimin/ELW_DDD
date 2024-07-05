using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Email_SMS_Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using IdentityService.Infrastructure;

namespace Microsoft.AspNetCore.Builder;

public static class IdentitySeriveExtensions
{
    /// <summary>
    /// 认证服务需要注册的服务
    /// </summary>
    /// <param name="builder"></param>
    public static void AddIdentityService(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        services.AddDataProtection();
        // 调用 AddIdentityCore 添加标识框架的一些重要的基础服务。
        // 注意：是调用 AddIdentityCore（适合前后端分离开发模式），不是调用 AddIdentity（适合传统的 MVC 开发模式）
        services.AddIdentityCore<User>(options =>
        {
            // 设置密码要求（降低标准）
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        });
        // 注册 UserManager、RoleManager 等服务
        var identityBuilder = new IdentityBuilder(typeof(User), typeof(Role), services);
        identityBuilder.AddEntityFrameworkStores<IdDbContext>()
            .AddDefaultTokenProviders()
            .AddRoleManager<RoleManager<Role>>()
            .AddUserManager<IdUserManager>();

        services.AddScoped<IdDomainService>();
        services.AddScoped<IIdRepository, IdRepository>();
        services.AddScoped<ISMSSender, MockSMSSender>();
        services.AddScoped<IEmailSender, MockEmailSender>();
    }
}