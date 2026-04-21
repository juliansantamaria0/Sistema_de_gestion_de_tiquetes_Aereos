namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ScheduledFlightEntityConfiguration : IEntityTypeConfiguration<ScheduledFlightEntity>
{
    public void Configure(EntityTypeBuilder<ScheduledFlightEntity> builder)
    {
        builder.ToTable("scheduled_flight");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("scheduled_flight_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.BaseFlightId)
               .HasColumnName("base_flight_id")
               .IsRequired();

        builder.Property(e => e.AircraftId)
               .HasColumnName("aircraft_id")
               .IsRequired();

        builder.Property(e => e.GateId)
               .HasColumnName("gate_id")
               .IsRequired(false);

        // Pomelo 8.x mapea DateOnly ↔ DATE de MySQL de forma nativa.
        builder.Property(e => e.DepartureDate)
               .HasColumnName("departure_date")
               .IsRequired();

        // Pomelo 8.x mapea TimeOnly ↔ TIME de MySQL de forma nativa.
        builder.Property(e => e.DepartureTime)
               .HasColumnName("departure_time")
               .IsRequired();

        builder.Property(e => e.EstimatedArrivalDatetime)
               .HasColumnName("estimated_arrival_datetime")
               .IsRequired();

        builder.Property(e => e.FlightStatusId)
               .HasColumnName("flight_status_id")
               .IsRequired();

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .IsRequired(false);

        // UNIQUE (base_flight_id, departure_date, departure_time) — espejo de uq_sf
        builder.HasIndex(e => new { e.BaseFlightId, e.DepartureDate, e.DepartureTime })
               .IsUnique()
               .HasDatabaseName("uq_sf");
    }
}
