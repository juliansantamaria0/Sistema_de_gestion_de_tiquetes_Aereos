namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
    {
        builder.ToTable("payment");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("payment_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ReservationId)
               .HasColumnName("reservation_id")
               .IsRequired(false);

        builder.Property(e => e.TicketId)
               .HasColumnName("ticket_id")
               .IsRequired(false);

        builder.Property(e => e.CurrencyId)
               .HasColumnName("currency_id")
               .IsRequired();

        builder.Property(e => e.PaymentDate)
               .HasColumnName("payment_date")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Amount)
               .HasColumnName("amount")
               .IsRequired()
               .HasColumnType("decimal(12,2)");

        builder.Property(e => e.PaymentStatusId)
               .HasColumnName("payment_status_id")
               .IsRequired();

        builder.Property(e => e.PaymentMethodId)
               .HasColumnName("payment_method_id")
               .IsRequired();

        builder.Property(e => e.TransactionReference)
               .HasColumnName("transaction_reference")
               .IsRequired(false)
               .HasMaxLength(100);

        builder.Property(e => e.RejectionReason)
               .HasColumnName("rejection_reason")
               .IsRequired(false)
               .HasMaxLength(250);

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .IsRequired(false);
    }
}
