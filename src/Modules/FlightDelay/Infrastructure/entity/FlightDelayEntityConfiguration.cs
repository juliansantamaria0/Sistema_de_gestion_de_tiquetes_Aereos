namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
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
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");builder.HasOne<ScheduledFlightEntity>()
               .WithMany()
               .HasForeignKey(e => e.ScheduledFlightId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fd_flight");

        builder.HasOne<DelayReasonEntity>()
               .WithMany()
               .HasForeignKey(e => e.DelayReasonId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fd_reason");}
}