namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;













public sealed class PassengerDiscountAggregate
{
    public PassengerDiscountId Id                  { get; private set; }
    public int                 ReservationDetailId { get; private set; }
    public int                 DiscountTypeId      { get; private set; }
    public decimal             AmountApplied       { get; private set; }

    private PassengerDiscountAggregate()
    {
        Id = null!;
    }

    public PassengerDiscountAggregate(
        PassengerDiscountId id,
        int                 reservationDetailId,
        int                 discountTypeId,
        decimal             amountApplied)
    {
        if (reservationDetailId <= 0)
            throw new ArgumentException(
                "ReservationDetailId must be a positive integer.", nameof(reservationDetailId));

        if (discountTypeId <= 0)
            throw new ArgumentException(
                "DiscountTypeId must be a positive integer.", nameof(discountTypeId));

        ValidateAmount(amountApplied);

        Id                  = id;
        ReservationDetailId = reservationDetailId;
        DiscountTypeId      = discountTypeId;
        AmountApplied       = amountApplied;
    }

    
    
    
    
    public void AdjustAmount(decimal newAmount)
    {
        ValidateAmount(newAmount);
        AmountApplied = newAmount;
    }

    

    private static void ValidateAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException(
                "AmountApplied must be >= 0. [IR-5]", nameof(amount));
    }
}
