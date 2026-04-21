namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;

/// <summary>
/// Equipaje adicional asociado a un tiquete.
/// SQL: ticket_baggage. [NC-2] id renombrado a ticket_baggage_id.
///
/// 4NF: (ticket_id, baggage_type_id) → quantity, fee_charged.
/// Sin MVD independientes — no viola 4NF.
/// UNIQUE: (ticket_id, baggage_type_id).
///
/// Invariantes:
///   - quantity > 0 (espejo de chk_tb_qty).
///   - fee_charged >= 0 (tarifa cobrada, no puede ser negativa).
///   - ticket_id y baggage_type_id son la clave de negocio — inmutables.
///
/// UpdateQuantity(): ajusta la cantidad de piezas de este tipo de equipaje.
/// fee_charged también puede actualizarse si el precio cambia al modificar la cantidad.
/// </summary>
public sealed class TicketBaggageAggregate
{
    public TicketBaggageId Id            { get; private set; }
    public int             TicketId      { get; private set; }
    public int             BaggageTypeId { get; private set; }
    public int             Quantity      { get; private set; }
    public decimal         FeeCharged    { get; private set; }

    private TicketBaggageAggregate()
    {
        Id = null!;
    }

    public TicketBaggageAggregate(
        TicketBaggageId id,
        int             ticketId,
        int             baggageTypeId,
        int             quantity,
        decimal         feeCharged)
    {
        if (ticketId <= 0)
            throw new ArgumentException(
                "TicketId must be a positive integer.", nameof(ticketId));

        if (baggageTypeId <= 0)
            throw new ArgumentException(
                "BaggageTypeId must be a positive integer.", nameof(baggageTypeId));

        ValidateQuantity(quantity);
        ValidateFeeCharged(feeCharged);

        Id            = id;
        TicketId      = ticketId;
        BaggageTypeId = baggageTypeId;
        Quantity      = quantity;
        FeeCharged    = feeCharged;
    }

    /// <summary>
    /// Actualiza la cantidad y tarifa cobrada para este tipo de equipaje.
    /// ticket_id y baggage_type_id son la clave de negocio — inmutables.
    /// </summary>
    public void UpdateQuantityAndFee(int quantity, decimal feeCharged)
    {
        ValidateQuantity(quantity);
        ValidateFeeCharged(feeCharged);

        Quantity   = quantity;
        FeeCharged = feeCharged;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than 0. [chk_tb_qty]", nameof(quantity));
    }

    private static void ValidateFeeCharged(decimal feeCharged)
    {
        if (feeCharged < 0)
            throw new ArgumentException(
                "FeeCharged must be >= 0.", nameof(feeCharged));
    }
}
