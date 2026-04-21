namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LoyaltyAccountEntityConfiguration : IEntityTypeConfiguration<LoyaltyAccountEntity>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccountEntity> builder)
    {
        builder.ToTable("loyalty_account");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("loyalty_account_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.PassengerId)
               .HasColumnName("passenger_id")
               .IsRequired();

        builder.Property(e => e.LoyaltyProgramId)
               .HasColumnName("loyalty_program_id")
               .IsRequired();

        // UNIQUE (passenger_id, loyalty_program_id) — espejo de uq_la
        builder.HasIndex(e => new { e.PassengerId, e.LoyaltyProgramId })
               .IsUnique()
               .HasDatabaseName("uq_la");

        builder.Property(e => e.LoyaltyTierId)
               .HasColumnName("loyalty_tier_id")
               .IsRequired();
        // FK compuesta activa hacia loyalty_tier
        builder.HasOne(e => e.LoyaltyTier)
               .WithMany(e => e.LoyaltyAccounts)
               .HasPrincipalKey(e => new { e.LoyaltyProgramId, e.Id })
               .HasForeignKey(e => new { e.LoyaltyProgramId, e.LoyaltyTierId });

        builder.Property(e => e.TotalMiles)
               .HasColumnName("total_miles")
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(e => e.AvailableMiles)
               .HasColumnName("available_miles")
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(e => e.JoinedAt)
               .HasColumnName("joined_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
