namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class DelayReasonEntityConfiguration : IEntityTypeConfiguration<DelayReasonEntity>
{
    public void Configure(EntityTypeBuilder<DelayReasonEntity> builder)
    {
        builder.ToTable("delay_reason");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("delay_reason_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(80);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_delay_reason_name");

        builder.Property(e => e.Category)
               .HasColumnName("category")
               .IsRequired()
               .HasMaxLength(50);
    }
}
