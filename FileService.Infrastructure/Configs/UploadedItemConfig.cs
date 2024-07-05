using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.Configs;
internal class UploadedItemConfig : IEntityTypeConfiguration<UploadedItem>
{
    public void Configure(EntityTypeBuilder<UploadedItem> builder)
    {
        /* 注意：
            在PostgreSQL中：会自动为该列创建一个唯一索引（Unique Index）以确保主键的唯一性。
                不需要显式地指定是聚合索引还是非聚合索引，因为主键约束会自动创建所需的唯一索引。
            在SQLserver中：会自动为该列创建一个唯一聚集索引（Unique Clustered Index），聚集索引决定了表中数据的物理存储顺序。
                需要取消聚合索引的话，应该显示取消聚合索引。
         */

        builder.ToTable("T_FS_UploadedItem");
        builder.Property(u => u.FileName).IsUnicode(true).HasMaxLength(1024);
        builder.Property(u => u.FileSHA256Hash).IsUnicode(false).HasMaxLength(64);
        //由于每次上传都要按照文件的大小和哈希值查找历史记录，因此设置文件大小和哈希值组成复合索引。
        builder.HasIndex(u => new { u.FileSizeInBytes, u.FileSHA256Hash });
    }
}