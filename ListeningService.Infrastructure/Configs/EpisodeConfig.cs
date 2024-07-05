using ListeningService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListeningService.Infrastructure.Configs;

internal class EpisodeConfig : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.ToTable("T_Listening_Episode");
        //builder.HasKey(e => e.Id).IsClustered(false); // sqlserver数据库启动这个配置
        builder.OwnsOneMultilingualString(e => e.Name);
        builder.Property(e => e.AudioUrl).IsRequired().HasMaxLength(1000).IsUnicode(true);
        builder.OwnsOne(e => e.Subtitle, nn =>
        {
            nn.Property(s => s.Content).IsRequired().HasMaxLength(int.MaxValue).IsUnicode(true);
            nn.Property(s => s.Type).IsRequired().HasMaxLength(10).IsUnicode(false);
        });
        builder.HasIndex(e => new { e.AlbumId, e.IsDeleted });
    }
}
