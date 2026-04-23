namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BaseFlightEntityConfiguration : IEntityTypeConfiguration<BaseFlightEntity>
{
    public void Configure(EntityTypeBuilder<BaseFlightEntity> builder)
    {
        builder.ToTable("base_flight");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("base_flight_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.FlightCode)
               .HasColumnName("flight_code")
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(e => e.AirlineId)
               .HasColumnName("airline_id")
               .IsRequired();

        builder.Property(e => e.RouteId)
               .HasColumnName("route_id")
               .IsRequired();

builder.Property(e => e.CreatedAt)
       .HasColumnName("created_at")
       .HasColumnType("datetime(6)")
       .IsRequired()
       .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .IsRequired(false);

        
        builder.HasIndex(e => new { e.FlightCode, e.AirlineId })
               .IsUnique()
               .HasDatabaseName("uq_base_flight");builder.HasOne<AirlineEntity>()
               .WithMany()
               .HasForeignKey(e => e.AirlineId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_bf_airline");

        builder.HasOne<RouteEntity>()
               .WithMany()
               .HasForeignKey(e => e.RouteId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_bf_route");}
}
