namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TicketStatusHistoryEntityConfiguration
    : IEntityTypeConfiguration<TicketStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<TicketStatusHistoryEntity> builder)
    {
        builder.ToTable("ticket_status_history");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("ticket_status_history_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.TicketId)
               .HasColumnName("ticket_id")
               .IsRequired();

        builder.Property(e => e.TicketStatusId)
               .HasColumnName("ticket_status_id")
               .IsRequired();

        builder.Property(e => e.ChangedAt)
               .HasColumnName("changed_at")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.Notes)
               .HasColumnName("notes")
               .IsRequired(false)
               .HasMaxLength(250);

        builder.HasIndex(e => new { e.TicketId, e.TicketStatusId, e.ChangedAt })
               .IsUnique()
               .HasDatabaseName("uq_tsh");
    }
}