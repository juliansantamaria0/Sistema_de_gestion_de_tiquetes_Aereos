namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.ToTable("permission");
        builder.HasKey(e => e.PermissionId);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(80);
        builder.HasIndex(e => e.Name).IsUnique();

        builder.Property(e => e.Description)
               .IsRequired(false)
               .HasMaxLength(200);
    }
}
