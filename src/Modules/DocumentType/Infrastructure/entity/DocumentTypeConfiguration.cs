namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentTypeEntity>
{
    public void Configure(EntityTypeBuilder<DocumentTypeEntity> builder)
    {
        builder.ToTable("document_type");
        builder.HasKey(e => e.DocumentTypeId);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(50);
        builder.HasIndex(e => e.Name).IsUnique();

        builder.Property(e => e.Code)
               .IsRequired()
               .HasMaxLength(10);
        builder.HasIndex(e => e.Code).IsUnique();
    }
}
