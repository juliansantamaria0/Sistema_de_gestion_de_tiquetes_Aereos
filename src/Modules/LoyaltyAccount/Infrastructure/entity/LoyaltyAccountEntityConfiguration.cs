namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
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

        builder.HasIndex(e => new { e.PassengerId, e.LoyaltyProgramId })
               .IsUnique()
               .HasDatabaseName("uq_la");

        builder.Property(e => e.LoyaltyTierId)
               .HasColumnName("loyalty_tier_id")
               .IsRequired();

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
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");builder.HasOne<PassengerEntity>()
               .WithMany()
               .HasForeignKey(e => e.PassengerId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_la_passenger");

        builder.HasOne<LoyaltyProgramEntity>()
               .WithMany()
               .HasForeignKey(e => e.LoyaltyProgramId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_la_program");

        
        
        
        
        builder.HasOne<LoyaltyTierEntity>()
               .WithMany()
               .HasForeignKey(e => e.LoyaltyTierId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_la_tier");}
}