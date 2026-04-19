namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CountryEntityConfiguration : IEntityTypeConfiguration<CountryEntity>
{
    public void Configure(EntityTypeBuilder<CountryEntity> builder)
    {
        builder.ToTable("country");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .HasColumnName("id")
               .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(100);
    }
}
