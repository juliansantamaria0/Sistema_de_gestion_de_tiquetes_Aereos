namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LoyaltyTierEntityConfiguration : IEntityTypeConfiguration<LoyaltyTierEntity>
{
    public void Configure(EntityTypeBuilder<LoyaltyTierEntity> builder)
    {
        builder.ToTable("loyalty_tier");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("loyalty_tier_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.LoyaltyProgramId)
               .HasColumnName("loyalty_program_id")
               .IsRequired();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        
        builder.HasIndex(e => new { e.LoyaltyProgramId, e.Name })
               .IsUnique()
               .HasDatabaseName("uq_lt");

        
        
        builder.HasIndex(e => new { e.LoyaltyProgramId, e.Id })
               .IsUnique()
               .HasDatabaseName("uq_lt_fk");

        builder.HasAlternateKey(e => new { e.LoyaltyProgramId, e.Id });

        builder.Property(e => e.MinMiles)
               .HasColumnName("min_miles")
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(e => e.Benefits)
               .HasColumnName("benefits")
               .IsRequired(false);

        
        builder.HasOne<LoyaltyProgramEntity>()
               .WithMany()
               .HasForeignKey(e => e.LoyaltyProgramId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_lt_program");
    }
}