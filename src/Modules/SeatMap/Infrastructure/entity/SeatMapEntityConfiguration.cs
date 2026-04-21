namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SeatMapEntityConfiguration : IEntityTypeConfiguration<SeatMapEntity>
{
    public void Configure(EntityTypeBuilder<SeatMapEntity> builder)
    {
        builder.ToTable("seat_map");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("seat_map_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.AircraftTypeId)
               .HasColumnName("aircraft_type_id")
               .IsRequired();

        builder.Property(e => e.SeatNumber)
               .HasColumnName("seat_number")
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(e => e.CabinClassId)
               .HasColumnName("cabin_class_id")
               .IsRequired();

        builder.Property(e => e.SeatFeatures)
               .HasColumnName("seat_features")
               .IsRequired(false)
               .HasMaxLength(100);

        // UNIQUE (aircraft_type_id, seat_number) — espejo de uq_seat_map
        builder.HasIndex(e => new { e.AircraftTypeId, e.SeatNumber })
               .IsUnique()
               .HasDatabaseName("uq_seat_map");
    }
}
