namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;

public sealed class CheckInEntityConfiguration : IEntityTypeConfiguration<CheckInEntity>
{
    public void Configure(EntityTypeBuilder<CheckInEntity> builder)
    {
        builder.ToTable("check_in");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("check_in_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.TicketId)
               .HasColumnName("ticket_id")
               .IsRequired();

        builder.HasIndex(e => e.TicketId)
               .IsUnique()
               .HasDatabaseName("uq_check_in_ticket");

        builder.Property(e => e.CheckInTime)
               .HasColumnName("check_in_time")
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.CheckInStatusId)
               .HasColumnName("check_in_status_id")
               .IsRequired();

        builder.Property(e => e.CounterNumber)
               .HasColumnName("counter_number")
               .IsRequired(false)
               .HasMaxLength(20);

        builder.HasOne<TicketEntity>()
               .WithMany()
               .HasForeignKey(e => e.TicketId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_ci_ticket");

        builder.HasOne<CheckInStatusEntity>()
               .WithMany()
               .HasForeignKey(e => e.CheckInStatusId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_ci_status");
    }
}
