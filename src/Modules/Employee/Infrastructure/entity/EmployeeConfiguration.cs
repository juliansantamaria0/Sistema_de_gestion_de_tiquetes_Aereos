namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("employee");
        builder.HasKey(e => e.EmployeeId);

        // person_id UNIQUE (un empleado por persona)
        builder.HasIndex(e => e.PersonId).IsUnique();

        builder.Property(e => e.HireDate).IsRequired();
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.JobPositionId).IsRequired(false);
        builder.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).IsRequired(false);
    }
}
