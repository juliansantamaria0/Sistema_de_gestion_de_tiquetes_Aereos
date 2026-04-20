namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>Configuración EF Core para <see cref="GateEntity"/>.</summary>
public sealed class GateConfiguration : IEntityTypeConfiguration<GateEntity>
{
    public void Configure(EntityTypeBuilder<GateEntity> builder)
    {
        builder.ToTable("gate");

        builder.HasKey(e => e.GateId);

        builder.Property(e => e.Code)
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(e => e.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // Índice único compuesto (terminal_id, code)
        builder.HasIndex(e => new { e.TerminalId, e.Code })
               .IsUnique()
               .HasDatabaseName("uq_gate");
    }
}
