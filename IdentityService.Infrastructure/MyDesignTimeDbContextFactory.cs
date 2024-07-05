using CommonInitializer;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityService.Infrastructure;

/// <summary>
/// 数据库迁移用到的脚本
/// </summary>
internal class MyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdDbContext>
{
    public IdDbContext CreateDbContext(string[] args)
    {
        var builder = DbContextOptionsBuilderFactory.Create<IdDbContext>();
        return new IdDbContext(builder.Options);
    }
}