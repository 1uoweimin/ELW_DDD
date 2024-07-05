using MediaEncoderService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaEncoderService.Infrastructure.Configs;

public class EncodingItemConfig : IEntityTypeConfiguration<EncodingItem>
{
    public void Configure(EntityTypeBuilder<EncodingItem> builder)
    {
        builder.ToTable("T_ME_EncodingItems");
        builder.Property(e => e.FileName).HasMaxLength(256).IsUnicode(false);
        builder.Property(e => e.FileSHA256Hash).HasMaxLength(64).IsUnicode(false);
        builder.Property(e => e.OutputFormat).HasMaxLength(10).IsUnicode(false);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(10);
    }
}