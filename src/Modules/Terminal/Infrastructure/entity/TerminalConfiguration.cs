namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>Configuración EF Core para <see cref="TerminalEntity"/>.</summary>
public sealed class TerminalConfiguration : IEntityTypeConfiguration<TerminalEntity>
{
    public void Configure(EntityTypeBuilder<TerminalEntity> builder)
    {
        builder.ToTable("terminal");

        builder.HasKey(e => e.TerminalId);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(e => e.IsInternational)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
               .HasColumnType("datetime(6)")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasIndex(e => new { e.AirportId, e.Name })
               .IsUnique()
               .HasDatabaseName("uq_terminal");builder.HasOne<AirportEntity>()
               .WithMany()
               .HasForeignKey(e => e.AirportId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_terminal_airport");}
}