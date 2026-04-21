namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ReservationEntityConfiguration : IEntityTypeConfiguration<ReservationEntity>
{
    public void Configure(EntityTypeBuilder<ReservationEntity> builder)
    {
        builder.ToTable("reservation");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("reservation_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ReservationCode)
               .HasColumnName("reservation_code")
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(e => e.ReservationCode)
               .IsUnique()
               .HasDatabaseName("uq_reservation_code");

        builder.Property(e => e.CustomerId)
               .HasColumnName("customer_id")
               .IsRequired();

        builder.Property(e => e.ScheduledFlightId)
               .HasColumnName("scheduled_flight_id")
               .IsRequired();

        builder.Property(e => e.ReservationDate)
               .HasColumnName("reservation_date")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ReservationStatusId)
               .HasColumnName("reservation_status_id")
               .IsRequired();

        builder.Property(e => e.ConfirmedAt)
               .HasColumnName("confirmed_at")
               .IsRequired(false);

        builder.Property(e => e.CancelledAt)
               .HasColumnName("cancelled_at")
               .IsRequired(false);

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .IsRequired(false);
    }
}
