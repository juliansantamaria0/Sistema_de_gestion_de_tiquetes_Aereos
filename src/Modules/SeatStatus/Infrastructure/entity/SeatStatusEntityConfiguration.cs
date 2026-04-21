namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SeatStatusEntityConfiguration : IEntityTypeConfiguration<SeatStatusEntity>
{
    public void Configure(EntityTypeBuilder<SeatStatusEntity> builder)
    {
        builder.ToTable("seat_status");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("seat_status_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_seat_status_name");
    }
}
