namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermissionEntity>
{
    public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
    {
        builder.ToTable("role_permission");
        builder.HasKey(e => e.RolePermissionId);

        
        builder.HasIndex(e => new { e.RoleId, e.PermissionId })
               .IsUnique()
               .HasDatabaseName("uq_rp");
    }
}
