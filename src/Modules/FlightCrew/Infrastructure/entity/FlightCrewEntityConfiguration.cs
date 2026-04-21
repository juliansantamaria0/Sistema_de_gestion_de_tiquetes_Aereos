namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class FlightCrewEntityConfiguration : IEntityTypeConfiguration<FlightCrewEntity>
{
    public void Configure(EntityTypeBuilder<FlightCrewEntity> builder)
    {
        builder.ToTable("flight_crew");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("flight_crew_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ScheduledFlightId)
               .HasColumnName("scheduled_flight_id")
               .IsRequired();

        builder.Property(e => e.EmployeeId)
               .HasColumnName("employee_id")
               .IsRequired();

        builder.Property(e => e.CrewRoleId)
               .HasColumnName("crew_role_id")
               .IsRequired();

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasIndex(e => new { e.ScheduledFlightId, e.EmployeeId })
               .IsUnique()
               .HasDatabaseName("uq_fc_employee");
    }
}