namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TicketEntityConfiguration : IEntityTypeConfiguration<TicketEntity>
{
    public void Configure(EntityTypeBuilder<TicketEntity> builder)
    {
        builder.ToTable("ticket");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("ticket_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.TicketCode)
               .HasColumnName("ticket_code")
               .IsRequired()
               .HasMaxLength(30);

        builder.HasIndex(e => e.TicketCode)
               .IsUnique()
               .HasDatabaseName("uq_ticket_code");

        builder.Property(e => e.ReservationDetailId)
               .HasColumnName("reservation_detail_id")
               .IsRequired();

        builder.HasIndex(e => e.ReservationDetailId)
               .IsUnique()
               .HasDatabaseName("uq_ticket_reservation_detail");

        builder.Property(e => e.IssueDate)
               .HasColumnName("issue_date")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.TicketStatusId)
               .HasColumnName("ticket_status_id")
               .IsRequired();

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("datetime(6)")
               .IsRequired(false);builder.HasOne<ReservationDetailEntity>()
               .WithMany()
               .HasForeignKey(e => e.ReservationDetailId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_ticket_detail");

        builder.HasOne<TicketStatusEntity>()
               .WithMany()
               .HasForeignKey(e => e.TicketStatusId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_ticket_status");}
}