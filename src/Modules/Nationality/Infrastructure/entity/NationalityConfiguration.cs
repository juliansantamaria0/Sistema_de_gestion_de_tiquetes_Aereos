namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class NationalityConfiguration : IEntityTypeConfiguration<NationalityEntity>
{
    public void Configure(EntityTypeBuilder<NationalityEntity> builder)
    {
        builder.ToTable("nationality");
        builder.HasKey(e => e.NationalityId);

        builder.Property(e => e.Demonym)
               .IsRequired()
               .HasMaxLength(80);

        // country_id UNIQUE: un país → una sola nacionalidad
        builder.HasIndex(e => e.CountryId)
               .IsUnique()
               .HasDatabaseName("uq_nationality_country");
    }
}
