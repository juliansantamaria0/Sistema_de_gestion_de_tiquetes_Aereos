namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class DiscountTypeEntityConfiguration : IEntityTypeConfiguration<DiscountTypeEntity>
{
    public void Configure(EntityTypeBuilder<DiscountTypeEntity> builder)
    {
        builder.ToTable("discount_type");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("discount_type_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_discount_type_name");

        builder.Property(e => e.Percentage)
               .HasColumnName("percentage")
               .IsRequired()
               .HasColumnType("decimal(5,2)");

        builder.Property(e => e.AgeMin)
               .HasColumnName("age_min")
               .IsRequired(false);

        builder.Property(e => e.AgeMax)
               .HasColumnName("age_max")
               .IsRequired(false);
    }
}
