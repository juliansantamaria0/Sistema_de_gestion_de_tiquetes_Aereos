namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;

/// <summary>
/// Descuento aplicado a un pasajero en una línea de reserva concreta.
/// SQL: passenger_discount. [NC-3] id renombrado a passenger_discount_id.
///
/// Invariantes:
///   - amount_applied >= 0 [IR-5] — espejo del CHECK SQL.
///   - UNIQUE (reservation_detail_id, discount_type_id): un tipo de descuento
///     solo puede aplicarse una vez por línea de reserva.
///
/// La única modificación válida es actualizar el monto aplicado (AdjustAmount).
/// reservation_detail_id y discount_type_id forman la clave de negocio — inmutables.
/// </summary>
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

    /// <summary>
    /// Ajusta el monto del descuento aplicado.
    /// reservation_detail_id y discount_type_id son la clave de negocio — inmutables.
    /// </summary>
    public void AdjustAmount(decimal newAmount)
    {
        ValidateAmount(newAmount);
        AmountApplied = newAmount;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException(
                "AmountApplied must be >= 0. [IR-5]", nameof(amount));
    }
}
