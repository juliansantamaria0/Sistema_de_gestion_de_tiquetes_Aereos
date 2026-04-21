namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ContactTypeEntityConfiguration : IEntityTypeConfiguration<ContactTypeEntity>
{
    public void Configure(EntityTypeBuilder<ContactTypeEntity> builder)
    {
        builder.ToTable("contact_type");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("contact_type_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_contact_type_name");
    }
}
