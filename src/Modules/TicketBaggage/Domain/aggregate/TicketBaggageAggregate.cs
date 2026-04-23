namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;

















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

    
    
    
    
    public void UpdateQuantityAndFee(int quantity, decimal feeCharged)
    {
        ValidateQuantity(quantity);
        ValidateFeeCharged(feeCharged);

        Quantity   = quantity;
        FeeCharged = feeCharged;
    }

    

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
