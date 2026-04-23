namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RouteScheduleEntityConfiguration : IEntityTypeConfiguration<RouteScheduleEntity>
{
    public void Configure(EntityTypeBuilder<RouteScheduleEntity> builder)
    {
        builder.ToTable("route_schedule");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("route_schedule_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.BaseFlightId)
               .HasColumnName("base_flight_id")
               .IsRequired();

        builder.Property(e => e.DayOfWeek)
               .HasColumnName("day_of_week")
               .IsRequired();

        
        builder.Property(e => e.DepartureTime)
               .HasColumnName("departure_time")
               .IsRequired();

        
        builder.HasIndex(e => new { e.BaseFlightId, e.DayOfWeek, e.DepartureTime })
               .IsUnique()
               .HasDatabaseName("uq_rs");builder.HasOne<BaseFlightEntity>()
               .WithMany()
               .HasForeignKey(e => e.BaseFlightId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_rs_base_flight");}
}
