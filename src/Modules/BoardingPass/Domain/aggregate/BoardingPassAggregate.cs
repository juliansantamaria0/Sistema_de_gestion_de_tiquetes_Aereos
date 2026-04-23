namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.ValueObject;














public sealed class BoardingPassAggregate
{
    public BoardingPassId Id             { get; private set; }
    public int            CheckInId      { get; private set; }
    public int?           GateId         { get; private set; }
    public string?        BoardingGroup  { get; private set; }
    public int            FlightSeatId   { get; private set; }

    private BoardingPassAggregate()
    {
        Id = null!;
    }

    public BoardingPassAggregate(
        BoardingPassId id,
        int            checkInId,
        int?           gateId,
        string?        boardingGroup,
        int            flightSeatId)
    {
        if (checkInId <= 0)
            throw new ArgumentException(
                "CheckInId must be a positive integer.", nameof(checkInId));

        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException(
                "GateId must be a positive integer when provided.", nameof(gateId));

        if (flightSeatId <= 0)
            throw new ArgumentException(
                "FlightSeatId must be a positive integer.", nameof(flightSeatId));

        ValidateBoardingGroup(boardingGroup);

        Id            = id;
        CheckInId     = checkInId;
        GateId        = gateId;
        BoardingGroup = boardingGroup?.Trim();
        FlightSeatId  = flightSeatId;
    }

    
    
    
    
    
    public void Update(int? gateId, string? boardingGroup)
    {
        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException(
                "GateId must be a positive integer when provided.", nameof(gateId));

        ValidateBoardingGroup(boardingGroup);

        GateId        = gateId;
        BoardingGroup = boardingGroup?.Trim();
    }

    

    private static void ValidateBoardingGroup(string? boardingGroup)
    {
        if (boardingGroup is not null && boardingGroup.Trim().Length > 10)
            throw new ArgumentException(
                "BoardingGroup cannot exceed 10 characters.", nameof(boardingGroup));
    }
}
