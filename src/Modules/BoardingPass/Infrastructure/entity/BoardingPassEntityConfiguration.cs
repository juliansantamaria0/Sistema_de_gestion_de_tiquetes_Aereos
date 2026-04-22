namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BoardingPassEntityConfiguration : IEntityTypeConfiguration<BoardingPassEntity>
{
    public void Configure(EntityTypeBuilder<BoardingPassEntity> builder)
    {
        builder.ToTable("boarding_pass");

        builder.HasKey(e => e.Id);

        // PK en SQL es boarding_pass_id [NC-1]
        builder.Property(e => e.Id)
               .HasColumnName("boarding_pass_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.CheckInId)
               .HasColumnName("check_in_id")
               .IsRequired();

        // UNIQUE (check_in_id) — un boarding pass por check-in
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

        // [IR-4] FK → flight_seat (reemplazó seat_confirmed VARCHAR)
        builder.Property(e => e.FlightSeatId)
               .HasColumnName("flight_seat_id")
               .IsRequired();builder.HasOne<CheckInEntity>()
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
               .HasConstraintName("fk_bp_flight_seat");}
}
