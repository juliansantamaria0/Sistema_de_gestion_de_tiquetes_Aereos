namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ReservationStatusHistoryEntityConfiguration
    : IEntityTypeConfiguration<ReservationStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<ReservationStatusHistoryEntity> builder)
    {
        builder.ToTable("reservation_status_history");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("reservation_status_history_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ReservationId)
               .HasColumnName("reservation_id")
               .IsRequired();

        builder.Property(e => e.ReservationStatusId)
               .HasColumnName("reservation_status_id")
               .IsRequired();

        builder.Property(e => e.ChangedAt)
               .HasColumnName("changed_at")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.Notes)
               .HasColumnName("notes")
               .IsRequired(false)
               .HasMaxLength(250);

        builder.HasIndex(e => new { e.ReservationId, e.ReservationStatusId, e.ChangedAt })
               .IsUnique()
               .HasDatabaseName("uq_rsh");builder.HasOne<ReservationEntity>()
               .WithMany()
               .HasForeignKey(e => e.ReservationId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_rsh_reservation");

        builder.HasOne<ReservationStatusEntity>()
               .WithMany()
               .HasForeignKey(e => e.ReservationStatusId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_rsh_status");}
}