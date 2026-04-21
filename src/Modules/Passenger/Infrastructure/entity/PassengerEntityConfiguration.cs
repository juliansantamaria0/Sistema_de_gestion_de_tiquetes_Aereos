namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PassengerEntityConfiguration : IEntityTypeConfiguration<PassengerEntity>
{
    public void Configure(EntityTypeBuilder<PassengerEntity> builder)
    {
        builder.ToTable("passenger");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("passenger_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.PersonId)
               .HasColumnName("person_id")
               .IsRequired();

        // UNIQUE (person_id) — un rol pasajero por persona
        builder.HasIndex(e => e.PersonId)
               .IsUnique()
               .HasDatabaseName("uq_passenger_person");

        builder.Property(e => e.FrequentFlyerNumber)
               .HasColumnName("frequent_flyer_number")
               .IsRequired(false)
               .HasMaxLength(30);

        // UNIQUE (frequent_flyer_number) cuando no es null
        builder.HasIndex(e => e.FrequentFlyerNumber)
               .IsUnique()
               .HasFilter("frequent_flyer_number IS NOT NULL")
               .HasDatabaseName("uq_passenger_ffn");

        builder.Property(e => e.NationalityId)
               .HasColumnName("nationality_id")
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
