namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;















public sealed class SeatMapAggregate
{
    public SeatMapId Id             { get; private set; }
    public int       AircraftTypeId { get; private set; }
    public string    SeatNumber     { get; private set; }
    public int       CabinClassId   { get; private set; }
    public string?   SeatFeatures   { get; private set; }

    private SeatMapAggregate()
    {
        Id         = null!;
        SeatNumber = null!;
    }

    public SeatMapAggregate(
        SeatMapId id,
        int       aircraftTypeId,
        string    seatNumber,
        int       cabinClassId,
        string?   seatFeatures = null)
    {
        if (aircraftTypeId <= 0)
            throw new ArgumentException(
                "AircraftTypeId must be a positive integer.", nameof(aircraftTypeId));

        ValidateSeatNumber(seatNumber);

        if (cabinClassId <= 0)
            throw new ArgumentException(
                "CabinClassId must be a positive integer.", nameof(cabinClassId));

        if (seatFeatures is not null && seatFeatures.Length > 100)
            throw new ArgumentException(
                "SeatFeatures cannot exceed 100 characters.", nameof(seatFeatures));

        Id             = id;
        AircraftTypeId = aircraftTypeId;
        SeatNumber     = seatNumber.Trim().ToUpperInvariant();
        CabinClassId   = cabinClassId;
        SeatFeatures   = seatFeatures?.Trim();
    }

    
    
    
    
    public void Update(int cabinClassId, string? seatFeatures)
    {
        if (cabinClassId <= 0)
            throw new ArgumentException(
                "CabinClassId must be a positive integer.", nameof(cabinClassId));

        if (seatFeatures is not null && seatFeatures.Length > 100)
            throw new ArgumentException(
                "SeatFeatures cannot exceed 100 characters.", nameof(seatFeatures));

        CabinClassId = cabinClassId;
        SeatFeatures = seatFeatures?.Trim();
    }

    

    private static void ValidateSeatNumber(string seatNumber)
    {
        if (string.IsNullOrWhiteSpace(seatNumber))
            throw new ArgumentException("SeatNumber cannot be empty.", nameof(seatNumber));

        if (seatNumber.Trim().Length > 10)
            throw new ArgumentException(
                "SeatNumber cannot exceed 10 characters.", nameof(seatNumber));
    }
}
