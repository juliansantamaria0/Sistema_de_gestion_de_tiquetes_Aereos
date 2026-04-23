namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


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

        
        builder.HasIndex(e => new { e.TerminalId, e.Code })
               .IsUnique()
               .HasDatabaseName("uq_gate");builder.HasOne<TerminalEntity>()
               .WithMany()
               .HasForeignKey(e => e.TerminalId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_gate_terminal");}
}
