namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("employee");
        builder.HasKey(e => e.EmployeeId);

        builder.HasIndex(e => e.PersonId).IsUnique();

        builder.Property(e => e.HireDate).IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.JobPositionId)
            .IsRequired(false);

        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime(6)")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime(6)")
            .IsRequired(false);builder.HasOne<PersonEntity>()
               .WithMany()
               .HasForeignKey(e => e.PersonId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_employee_person");

        builder.HasOne<AirlineEntity>()
               .WithMany()
               .HasForeignKey(e => e.AirlineId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_employee_airline");

        builder.HasOne<JobPositionEntity>()
               .WithMany()
               .HasForeignKey(e => e.JobPositionId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_employee_position");}
}