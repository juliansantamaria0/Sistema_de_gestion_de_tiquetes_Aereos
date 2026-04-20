namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FlightDelayEntityConfiguration : IEntityTypeConfiguration<FlightDelayEntity>
{
    public void Configure(EntityTypeBuilder<FlightDelayEntity> builder)
    {
        builder.ToTable("flight_delay");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("flight_delay_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ScheduledFlightId)
               .HasColumnName("scheduled_flight_id")
               .IsRequired();

        builder.Property(e => e.DelayReasonId)
               .HasColumnName("delay_reason_id")
               .IsRequired();

        builder.Property(e => e.DelayMinutes)
               .HasColumnName("delay_minutes")
               .IsRequired();

        builder.Property(e => e.ReportedAt)
               .HasColumnName("reported_at")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
    }
}