namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PassengerContactEntityConfiguration
    : IEntityTypeConfiguration<PassengerContactEntity>
{
    public void Configure(EntityTypeBuilder<PassengerContactEntity> builder)
    {
        builder.ToTable("passenger_contact");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("passenger_contact_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.PassengerId)
               .HasColumnName("passenger_id")
               .IsRequired();

        builder.Property(e => e.ContactTypeId)
               .HasColumnName("contact_type_id")
               .IsRequired();

        
        builder.HasIndex(e => new { e.PassengerId, e.ContactTypeId })
               .IsUnique()
               .HasDatabaseName("uq_pc");

        builder.Property(e => e.FullName)
               .HasColumnName("full_name")
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.Phone)
               .HasColumnName("phone")
               .IsRequired()
               .HasMaxLength(30);

        builder.Property(e => e.Relationship)
               .HasColumnName("relationship")
               .IsRequired(false)
               .HasMaxLength(50);builder.HasOne<PassengerEntity>()
               .WithMany()
               .HasForeignKey(e => e.PassengerId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_pc_passenger");

        builder.HasOne<ContactTypeEntity>()
               .WithMany()
               .HasForeignKey(e => e.ContactTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_pc_contact_type");}
}
