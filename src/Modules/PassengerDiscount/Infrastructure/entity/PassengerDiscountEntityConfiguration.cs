namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity; using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Entity; using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PassengerDiscountEntityConfiguration : IEntityTypeConfiguration<PassengerDiscountEntity>
{
    public void Configure(EntityTypeBuilder<PassengerDiscountEntity> builder)
    {
        builder.ToTable("passenger_discount");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("passenger_discount_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.ReservationDetailId)
               .HasColumnName("reservation_detail_id")
               .IsRequired();

        builder.Property(e => e.DiscountTypeId)
               .HasColumnName("discount_type_id")
               .IsRequired();

        builder.Property(e => e.AmountApplied)
               .HasColumnName("amount_applied")
               .IsRequired()
               .HasColumnType("decimal(12,2)");

        
        builder.HasIndex(e => new { e.ReservationDetailId, e.DiscountTypeId })
               .IsUnique()
               .HasDatabaseName("uq_pd");builder.HasOne<ReservationDetailEntity>()
               .WithMany()
               .HasForeignKey(e => e.ReservationDetailId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_pd_detail");

        builder.HasOne<DiscountTypeEntity>()
               .WithMany()
               .HasForeignKey(e => e.DiscountTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_pd_discount");}
}
