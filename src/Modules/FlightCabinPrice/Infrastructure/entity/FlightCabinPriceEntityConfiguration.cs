namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FlightCabinPriceEntityConfiguration
    : IEntityTypeConfiguration<FlightCabinPriceEntity>
{
    public void Configure(EntityTypeBuilder<FlightCabinPriceEntity> builder)
    {
        builder.ToTable("flight_cabin_price");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("flight_cabin_price_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ScheduledFlightId)
               .HasColumnName("scheduled_flight_id")
               .IsRequired();

        builder.Property(e => e.CabinClassId)
               .HasColumnName("cabin_class_id")
               .IsRequired();

        builder.Property(e => e.FareTypeId)
               .HasColumnName("fare_type_id")
               .IsRequired();

        // UNIQUE (scheduled_flight_id, cabin_class_id, fare_type_id) — espejo de uq_fcp
        builder.HasIndex(e => new { e.ScheduledFlightId, e.CabinClassId, e.FareTypeId })
               .IsUnique()
               .HasDatabaseName("uq_fcp");

        builder.Property(e => e.Price)
               .HasColumnName("price")
               .IsRequired()
               .HasColumnType("decimal(12,2)");builder.HasOne<ScheduledFlightEntity>()
               .WithMany()
               .HasForeignKey(e => e.ScheduledFlightId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fcp_flight");

        builder.HasOne<CabinClassEntity>()
               .WithMany()
               .HasForeignKey(e => e.CabinClassId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fcp_cabin");

        builder.HasOne<FareTypeEntity>()
               .WithMany()
               .HasForeignKey(e => e.FareTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fcp_fare_type");}
}
