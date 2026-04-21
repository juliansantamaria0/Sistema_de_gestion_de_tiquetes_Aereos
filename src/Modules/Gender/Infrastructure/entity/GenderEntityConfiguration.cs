namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class GenderEntityConfiguration : IEntityTypeConfiguration<GenderEntity>
{
    public void Configure(EntityTypeBuilder<GenderEntity> builder)
    {
        builder.ToTable("gender");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("gender_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_gender_name");
    }
}
