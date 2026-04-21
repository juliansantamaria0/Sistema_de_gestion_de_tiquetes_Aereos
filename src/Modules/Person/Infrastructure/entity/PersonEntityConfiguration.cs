namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PersonEntityConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("person");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("person_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.DocumentTypeId)
               .HasColumnName("document_type_id")
               .IsRequired();

        builder.Property(e => e.DocumentNumber)
               .HasColumnName("document_number")
               .IsRequired()
               .HasMaxLength(30);

        // UNIQUE (document_type_id, document_number) — espejo de uq_person_document
        builder.HasIndex(e => new { e.DocumentTypeId, e.DocumentNumber })
               .IsUnique()
               .HasDatabaseName("uq_person_document");

        builder.Property(e => e.FirstName)
               .HasColumnName("first_name")
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.LastName)
               .HasColumnName("last_name")
               .IsRequired()
               .HasMaxLength(100);

        // DateOnly? — columna DATE en MySQL, Pomelo 8.x mapea nativamente
        builder.Property(e => e.BirthDate)
               .HasColumnName("birth_date")
               .IsRequired(false);

        builder.Property(e => e.GenderId)
               .HasColumnName("gender_id")
               .IsRequired(false);

        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .IsRequired(false);
    }
}
