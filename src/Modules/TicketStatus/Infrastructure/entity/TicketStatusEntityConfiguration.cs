namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TicketStatusEntityConfiguration : IEntityTypeConfiguration<TicketStatusEntity>
{
    public void Configure(EntityTypeBuilder<TicketStatusEntity> builder)
    {
        builder.ToTable("ticket_status");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("ticket_status_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_ticket_status_name");
    }
}
