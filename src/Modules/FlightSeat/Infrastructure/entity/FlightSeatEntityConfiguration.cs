namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FlightSeatEntityConfiguration : IEntityTypeConfiguration<FlightSeatEntity>
{
    public void Configure(EntityTypeBuilder<FlightSeatEntity> builder)
    {
        builder.ToTable("flight_seat");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("flight_seat_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ScheduledFlightId)
               .HasColumnName("scheduled_flight_id")
               .IsRequired();

        builder.Property(e => e.SeatMapId)
               .HasColumnName("seat_map_id")
               .IsRequired();

        builder.Property(e => e.SeatStatusId)
               .HasColumnName("seat_status_id")
               .IsRequired();

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .IsRequired(false);

        // UNIQUE (scheduled_flight_id, seat_map_id) — espejo de uq_flight_seat
        builder.HasIndex(e => new { e.ScheduledFlightId, e.SeatMapId })
               .IsUnique()
               .HasDatabaseName("uq_flight_seat");
    }
}
