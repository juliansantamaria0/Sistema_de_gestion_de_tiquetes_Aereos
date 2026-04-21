namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RouteEntityConfiguration : IEntityTypeConfiguration<RouteEntity>
{
    public void Configure(EntityTypeBuilder<RouteEntity> builder)
    {
        builder.ToTable("route");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("route_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.OriginAirportId)
               .HasColumnName("origin_airport_id")
               .IsRequired();

        builder.Property(e => e.DestinationAirportId)
               .HasColumnName("destination_airport_id")
               .IsRequired();

        // UNIQUE (origin_airport_id, destination_airport_id) — espejo de uq_route
        builder.HasIndex(e => new { e.OriginAirportId, e.DestinationAirportId })
               .IsUnique()
               .HasDatabaseName("uq_route");

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
