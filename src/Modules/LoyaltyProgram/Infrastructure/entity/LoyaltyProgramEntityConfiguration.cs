namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LoyaltyProgramEntityConfiguration : IEntityTypeConfiguration<LoyaltyProgramEntity>
{
    public void Configure(EntityTypeBuilder<LoyaltyProgramEntity> builder)
    {
        builder.ToTable("loyalty_program");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("loyalty_program_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.AirlineId)
               .HasColumnName("airline_id")
               .IsRequired();

        builder.HasIndex(e => e.AirlineId)
               .IsUnique()
               .HasDatabaseName("uq_loyalty_program_airline");

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_loyalty_program_name");

        builder.Property(e => e.MilesPerDollar)
               .HasColumnName("miles_per_dollar")
               .IsRequired()
               .HasColumnType("decimal(6,2)")
               .HasDefaultValue(1m);builder.HasOne<AirlineEntity>()
               .WithMany()
               .HasForeignKey(e => e.AirlineId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_lp_airline");}
}
