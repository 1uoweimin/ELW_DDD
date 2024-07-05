using CommonInitializer;
using Microsoft.EntityFrameworkCore.Design;

namespace FileService.Infrastructure;

/// <summary>
/// 数据库迁移用到的脚本
/// </summary>
internal class MyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<FSDbContext>
{
    public FSDbContext CreateDbContext(string[] args)
    {
        var builder = DbContextOptionsBuilderFactory.Create<FSDbContext>();
        return new FSDbContext(builder.Options, null);
    }
}
