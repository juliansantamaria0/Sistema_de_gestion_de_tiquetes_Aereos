namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CabinClassEntityConfiguration : IEntityTypeConfiguration<CabinClassEntity>
{
    public void Configure(EntityTypeBuilder<CabinClassEntity> builder)
    {
        builder.ToTable("cabin_class");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("cabin_class_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_cabin_class_name");
    }
}
