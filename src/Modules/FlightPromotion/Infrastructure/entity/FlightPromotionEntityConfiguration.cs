namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FlightPromotionEntityConfiguration
    : IEntityTypeConfiguration<FlightPromotionEntity>
{
    public void Configure(EntityTypeBuilder<FlightPromotionEntity> builder)
    {
        builder.ToTable("flight_promotion");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("flight_promotion_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ScheduledFlightId)
               .HasColumnName("scheduled_flight_id")
               .IsRequired();

        builder.Property(e => e.PromotionId)
               .HasColumnName("promotion_id")
               .IsRequired();

        // UNIQUE (scheduled_flight_id, promotion_id) — espejo de uq_fp
        builder.HasIndex(e => new { e.ScheduledFlightId, e.PromotionId })
               .IsUnique()
               .HasDatabaseName("uq_fp");builder.HasOne<ScheduledFlightEntity>()
               .WithMany()
               .HasForeignKey(e => e.ScheduledFlightId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fp_flight");

        builder.HasOne<PromotionEntity>()
               .WithMany()
               .HasForeignKey(e => e.PromotionId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fp_promotion");}
}
