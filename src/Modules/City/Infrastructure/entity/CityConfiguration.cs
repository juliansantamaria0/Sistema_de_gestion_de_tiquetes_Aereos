namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Infrastructure.Entity; 




public sealed class CityConfiguration : IEntityTypeConfiguration<CityEntity>
{
       public void Configure(EntityTypeBuilder<CityEntity> builder)
       {
              builder.ToTable("city");

              builder.HasKey(e => e.CityId);

              builder.Property(e => e.Name)
                     .IsRequired()
                     .HasMaxLength(100);

              builder.Property(e => e.CreatedAt)
                     .HasColumnType("datetime(6)")
                     .IsRequired()
                     .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

              builder.HasIndex(e => new { e.Name, e.CountryId })
                     .IsUnique()
                     .HasDatabaseName("uq_city");

              builder.HasOne<CountryEntity>()
                     .WithMany()
                     .HasForeignKey(e => e.CountryId)
                     .OnDelete(DeleteBehavior.Restrict)
                     .HasConstraintName("fk_city_country");
       }
}