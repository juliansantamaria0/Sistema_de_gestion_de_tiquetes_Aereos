namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;



















public sealed class FlightCabinPriceAggregate
{
    public FlightCabinPriceId Id                { get; private set; }
    public int                ScheduledFlightId { get; private set; }
    public int                CabinClassId      { get; private set; }
    public int                FareTypeId        { get; private set; }
    public decimal            Price             { get; private set; }

    private FlightCabinPriceAggregate()
    {
        Id = null!;
    }

    public FlightCabinPriceAggregate(
        FlightCabinPriceId id,
        int                scheduledFlightId,
        int                cabinClassId,
        int                fareTypeId,
        decimal            price)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (cabinClassId <= 0)
            throw new ArgumentException(
                "CabinClassId must be a positive integer.", nameof(cabinClassId));

        if (fareTypeId <= 0)
            throw new ArgumentException(
                "FareTypeId must be a positive integer.", nameof(fareTypeId));

        ValidatePrice(price);

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        CabinClassId      = cabinClassId;
        FareTypeId        = fareTypeId;
        Price             = price;
    }

    
    
    
    
    public void UpdatePrice(decimal newPrice)
    {
        ValidatePrice(newPrice);
        Price = newPrice;
    }

    

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Price must be >= 0. [chk_fcp_price]", nameof(price));
    }
}
