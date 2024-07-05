using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure;

public class IdDbContext : IdentityDbContext<User, Role, Guid>
{
    public IdDbContext(DbContextOptions<IdDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        builder.EnableSoftDeletionGlobalFilter(); //启动软删除
        builder.EnableConvertToUTCGlobal();

        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("T_Id_RoleClaim");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("T_Id_UserClaim");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("T_Id_UserLogin");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("T_Id_UserRole");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("T_Id_UserToken");
    }
}