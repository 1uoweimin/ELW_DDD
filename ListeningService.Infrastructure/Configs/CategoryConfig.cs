using ListeningService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListeningService.Infrastructure.Configs;

internal class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("T_Listening_Categories");
        //builder.HasKey(c => c.Id).IsClustered(false); // sqlserver数据库启动这个配置
        builder.OwnsOneMultilingualString(c => c.Name);
        builder.Property(c => c.CoverUrl).IsRequired(false).HasMaxLength(500).IsUnicode(true);
    }
}
