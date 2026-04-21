namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ReservationStatusEntityConfiguration : IEntityTypeConfiguration<ReservationStatusEntity>
{
    public void Configure(EntityTypeBuilder<ReservationStatusEntity> builder)
    {
        builder.ToTable("reservation_status");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("reservation_status_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_reservation_status_name");
    }
}
