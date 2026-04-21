namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FareTypeEntityConfiguration : IEntityTypeConfiguration<FareTypeEntity>
{
    public void Configure(EntityTypeBuilder<FareTypeEntity> builder)
    {
        builder.ToTable("fare_type");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("fare_type_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_fare_type_name");

        builder.Property(e => e.IsRefundable)
               .HasColumnName("is_refundable")
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(e => e.IsChangeable)
               .HasColumnName("is_changeable")
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(e => e.AdvancePurchaseDays)
               .HasColumnName("advance_purchase_days")
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(e => e.BaggageIncluded)
               .HasColumnName("baggage_included")
               .IsRequired()
               .HasDefaultValue(false);
    }
}
