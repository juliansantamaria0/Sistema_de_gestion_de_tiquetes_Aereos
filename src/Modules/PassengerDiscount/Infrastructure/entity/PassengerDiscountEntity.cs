namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Infrastructure.Entity;

public sealed class PassengerDiscountEntity
{
    public int     Id                  { get; set; }
    public int     ReservationDetailId { get; set; }
    public int     DiscountTypeId      { get; set; }
    public decimal AmountApplied       { get; set; }
}
