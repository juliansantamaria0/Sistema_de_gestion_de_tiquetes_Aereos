namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("user");
        builder.HasKey(e => e.UserId);

        builder.Property(e => e.Username)
               .IsRequired()
               .HasMaxLength(60);

        builder.HasIndex(e => e.Username)
               .IsUnique();

        builder.HasIndex(e => e.PersonId)
               .IsUnique();

        builder.Property(e => e.PasswordHash)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(e => e.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.UpdatedAt)
               .HasColumnType("datetime(6)")
               .IsRequired(false);
    }
}