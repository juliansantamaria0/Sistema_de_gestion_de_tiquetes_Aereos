namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ReservationDetailEntityConfiguration : IEntityTypeConfiguration<ReservationDetailEntity>
{
    public void Configure(EntityTypeBuilder<ReservationDetailEntity> builder)
    {
        builder.ToTable("reservation_detail");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("reservation_detail_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ReservationId)
               .HasColumnName("reservation_id")
               .IsRequired();

        builder.Property(e => e.PassengerId)
               .HasColumnName("passenger_id")
               .IsRequired();

        builder.Property(e => e.FlightSeatId)
               .HasColumnName("flight_seat_id")
               .IsRequired();

        builder.Property(e => e.FareTypeId)
               .HasColumnName("fare_type_id")
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

        builder.HasIndex(e => new { e.ReservationId, e.PassengerId })
               .IsUnique()
               .HasDatabaseName("uq_rd_passenger");

        builder.HasIndex(e => new { e.ReservationId, e.FlightSeatId })
               .IsUnique()
               .HasDatabaseName("uq_rd_seat");builder.HasOne<ReservationEntity>()
               .WithMany()
               .HasForeignKey(e => e.ReservationId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_rd_reservation");

        builder.HasOne<PassengerEntity>()
               .WithMany()
               .HasForeignKey(e => e.PassengerId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_rd_passenger");

        builder.HasOne<FlightSeatEntity>()
               .WithMany()
               .HasForeignKey(e => e.FlightSeatId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_rd_seat");

        builder.HasOne<FareTypeEntity>()
               .WithMany()
               .HasForeignKey(e => e.FareTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_rd_fare_type");}
}