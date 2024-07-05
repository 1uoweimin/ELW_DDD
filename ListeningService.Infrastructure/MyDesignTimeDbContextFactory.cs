using CommonInitializer;
using Microsoft.EntityFrameworkCore.Design;

namespace ListeningService.Infrastructure;

/// <summary>
/// 数据库迁移用到的脚本
/// </summary>
internal class MyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ListeningDbContext>
{
    public ListeningDbContext CreateDbContext(string[] args)
    {
        var builder = DbContextOptionsBuilderFactory.Create<ListeningDbContext>();
        return new ListeningDbContext(builder.Options, null);
    }
}