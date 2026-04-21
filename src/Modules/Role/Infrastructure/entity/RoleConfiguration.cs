namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("role");
        builder.HasKey(e => e.RoleId);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(50);
        builder.HasIndex(e => e.Name).IsUnique();

        builder.Property(e => e.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
    }
}
