namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;

public sealed class BoardingPassEntityConfiguration : IEntityTypeConfiguration<BoardingPassEntity>
{
    public void Configure(EntityTypeBuilder<BoardingPassEntity> builder)
    {
        builder.ToTable("boarding_pass");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("boarding_pass_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.BoardingPassCode)
               .HasColumnName("boarding_pass_code")
               .IsRequired()
               .HasMaxLength(30);

        builder.HasIndex(e => e.BoardingPassCode)
               .IsUnique()
               .HasDatabaseName("uq_boarding_pass_code");

        builder.Property(e => e.CheckInId)
               .HasColumnName("check_in_id")
               .IsRequired();

        builder.HasIndex(e => e.CheckInId)
               .IsUnique()
               .HasDatabaseName("uq_boarding_pass_check_in");

        builder.Property(e => e.GateId)
               .HasColumnName("gate_id")
               .IsRequired(false);

        builder.Property(e => e.BoardingGroup)
               .HasColumnName("boarding_group")
               .IsRequired(false)
               .HasMaxLength(10);

        builder.Property(e => e.FlightSeatId)
               .HasColumnName("flight_seat_id")
               .IsRequired();

        builder.Property(e => e.IssuedAt)
               .HasColumnName("issued_at")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasOne<CheckInEntity>()
               .WithMany()
               .HasForeignKey(e => e.CheckInId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_bp_check_in");

        builder.HasOne<GateEntity>()
               .WithMany()
               .HasForeignKey(e => e.GateId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_bp_gate");

        builder.HasOne<FlightSeatEntity>()
               .WithMany()
               .HasForeignKey(e => e.FlightSeatId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_bp_flight_seat");
    }
}
