namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LoyaltyTransactionEntityConfiguration
    : IEntityTypeConfiguration<LoyaltyTransactionEntity>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransactionEntity> builder)
    {
        builder.ToTable("loyalty_transaction");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("loyalty_transaction_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.LoyaltyAccountId)
               .HasColumnName("loyalty_account_id")
               .IsRequired();

        builder.Property(e => e.TicketId)
               .HasColumnName("ticket_id")
               .IsRequired(false);

        builder.Property(e => e.TransactionType)
               .HasColumnName("transaction_type")
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(e => e.Miles)
               .HasColumnName("miles")
               .IsRequired();

        builder.Property(e => e.TransactionDate)
               .HasColumnName("transaction_date")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
