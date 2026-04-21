namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FlightStatusHistoryEntityConfiguration
    : IEntityTypeConfiguration<FlightStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<FlightStatusHistoryEntity> builder)
    {
        builder.ToTable("flight_status_history");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("flight_status_history_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ScheduledFlightId)
               .HasColumnName("scheduled_flight_id")
               .IsRequired();

        builder.Property(e => e.FlightStatusId)
               .HasColumnName("flight_status_id")
               .IsRequired();

        builder.Property(e => e.ChangedAt)
               .HasColumnName("changed_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Notes)
               .HasColumnName("notes")
               .IsRequired(false)
               .HasMaxLength(250);

        // UNIQUE (scheduled_flight_id, flight_status_id, changed_at) — espejo de uq_fsh
        builder.HasIndex(e => new { e.ScheduledFlightId, e.FlightStatusId, e.ChangedAt })
               .IsUnique()
               .HasDatabaseName("uq_fsh");
    }
}
