namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Infrastructure.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethodEntity>
{
    public void Configure(EntityTypeBuilder<PaymentMethodEntity> builder)
    {
        builder.ToTable("payment_method");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("payment_method_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
               .HasColumnName("name")
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(e => e.Name)
               .IsUnique()
               .HasDatabaseName("uq_payment_method_name");
    }
}
