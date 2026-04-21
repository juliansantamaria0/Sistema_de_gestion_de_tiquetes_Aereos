namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PromotionEntityConfiguration : IEntityTypeConfiguration<PromotionEntity>
{
    public void Configure(EntityTypeBuilder<PromotionEntity> builder)
    {
        builder.ToTable("promotion");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("promotion_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.AirlineId)
               .HasColumnName("airline_id")
               .IsRequired();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.DiscountPct)
               .HasColumnName("discount_pct")
               .IsRequired()
               .HasColumnType("decimal(5,2)");

        // DateOnly — columnas DATE de MySQL, Pomelo 8.x mapea nativamente
        builder.Property(e => e.ValidFrom)
               .HasColumnName("valid_from")
               .IsRequired();

        builder.Property(e => e.ValidUntil)
               .HasColumnName("valid_until")
               .IsRequired();
    }
}
