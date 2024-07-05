using CommonInitializer;
using MediaEncoderService.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;

namespace ListeningService.Infrastructure;

/// <summary>
/// 数据库迁移用到的脚本
/// </summary>
internal class MyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MediaEncoderDbContext>
{
    public MediaEncoderDbContext CreateDbContext(string[] args)
    {
        var builder = DbContextOptionsBuilderFactory.Create<MediaEncoderDbContext>();
        return new MediaEncoderDbContext(builder.Options, null);
    }
}