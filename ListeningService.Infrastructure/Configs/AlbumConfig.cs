using ListeningService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListeningService.Infrastructure.Configs;

internal class AlbumConfig : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.ToTable("T_Listening_Album");
        //builder.HasKey(a => a.Id).IsClustered(false); // sqlserver数据库启动这个配置
        builder.OwnsOneMultilingualString(a => a.Name);
        builder.HasIndex(a => new { a.CategoryId, a.IsDeleted });
    }
}
