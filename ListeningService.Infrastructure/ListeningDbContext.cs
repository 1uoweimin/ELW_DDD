using ListeningService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zack.Infrastructure.EFCore;

namespace ListeningService.Infrastructure;

public class ListeningDbContext : BaseDbContext
{
    public DbSet<Category> Category { get; private set; }
    public DbSet<Album> Album { get; private set; }
    public DbSet<Episode> Episode { get; private set; }
    public ListeningDbContext(DbContextOptions options, IMediator? mediator) : base(options, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        modelBuilder.EnableSoftDeletionGlobalFilter();
        modelBuilder.EnableConvertToUTCGlobal();
    }
}
