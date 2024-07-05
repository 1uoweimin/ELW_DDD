using FileService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zack.Infrastructure.EFCore;

namespace FileService.Infrastructure;

/// <summary>
/// 文件服务器上下文
/// </summary>
public class FSDbContext : BaseDbContext
{
    public DbSet<UploadedItem> UploadedItems { get; private set; }
    public FSDbContext(DbContextOptions<FSDbContext> options, IMediator? mediator) : base(options, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        modelBuilder.EnableConvertToUTCGlobal();
    }
}