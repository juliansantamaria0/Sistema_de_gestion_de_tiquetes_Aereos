namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class JobPositionConfiguration : IEntityTypeConfiguration<JobPositionEntity>
{
    public void Configure(EntityTypeBuilder<JobPositionEntity> builder)
    {
        builder.ToTable("job_position");
        builder.HasKey(e => e.JobPositionId);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(80);
        builder.HasIndex(e => e.Name).IsUnique();

        builder.Property(e => e.Department)
               .IsRequired(false)
               .HasMaxLength(80);
    }
}
