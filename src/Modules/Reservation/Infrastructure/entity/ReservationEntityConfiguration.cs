namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
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
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.ReservationStatusId)
               .HasColumnName("reservation_status_id")
               .IsRequired();

        builder.Property(e => e.ConfirmedAt)
               .HasColumnName("confirmed_at")
               .HasColumnType("datetime(6)")
               .IsRequired(false);

        builder.Property(e => e.CancelledAt)
               .HasColumnName("cancelled_at")
               .HasColumnType("datetime(6)")
               .IsRequired(false);

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("datetime(6)")
               .IsRequired(false);builder.HasOne<CustomerEntity>()
               .WithMany()
               .HasForeignKey(e => e.CustomerId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_res_customer");

        builder.HasOne<ScheduledFlightEntity>()
               .WithMany()
               .HasForeignKey(e => e.ScheduledFlightId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_res_flight");

        builder.HasOne<ReservationStatusEntity>()
               .WithMany()
               .HasForeignKey(e => e.ReservationStatusId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_res_status");}
}