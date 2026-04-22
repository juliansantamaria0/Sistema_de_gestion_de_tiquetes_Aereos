namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
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
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("datetime(6)")
               .IsRequired(false);

        builder.HasIndex(e => new { e.ScheduledFlightId, e.SeatMapId })
               .IsUnique()
               .HasDatabaseName("uq_flight_seat");builder.HasOne<ScheduledFlightEntity>()
               .WithMany()
               .HasForeignKey(e => e.ScheduledFlightId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fs_flight");

        builder.HasOne<SeatMapEntity>()
               .WithMany()
               .HasForeignKey(e => e.SeatMapId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fs_seat_map");

        builder.HasOne<SeatStatusEntity>()
               .WithMany()
               .HasForeignKey(e => e.SeatStatusId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_fs_status");}
}