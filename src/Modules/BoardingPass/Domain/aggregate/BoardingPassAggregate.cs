namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.ValueObject;

public sealed class BoardingPassAggregate
{
    public BoardingPassId Id                { get; private set; }
    public string         BoardingPassCode  { get; private set; }
    public int            CheckInId         { get; private set; }
    public int?           GateId            { get; private set; }
    public string?        BoardingGroup     { get; private set; }
    public int            FlightSeatId      { get; private set; }
    public DateTime       IssuedAt          { get; private set; }

    private BoardingPassAggregate()
    {
        Id               = null!;
        BoardingPassCode = null!;
    }

    public BoardingPassAggregate(
        BoardingPassId id,
        string         boardingPassCode,
        int            checkInId,
        int?           gateId,
        string?        boardingGroup,
        int            flightSeatId,
        DateTime       issuedAt)
    {
        ValidateBoardingPassCode(boardingPassCode);

        if (checkInId <= 0)
            throw new ArgumentException(
                "El CheckInId debe ser un entero positivo.", nameof(checkInId));

        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException(
                "El GateId debe ser un entero positivo cuando se proporciona.", nameof(gateId));

        if (flightSeatId <= 0)
            throw new ArgumentException(
                "El FlightSeatId debe ser un entero positivo.", nameof(flightSeatId));

        ValidateBoardingGroup(boardingGroup);

        Id               = id;
        BoardingPassCode = boardingPassCode.Trim().ToUpperInvariant();
        CheckInId        = checkInId;
        GateId           = gateId;
        BoardingGroup    = boardingGroup?.Trim();
        FlightSeatId     = flightSeatId;
        IssuedAt         = issuedAt;
    }

    public void Update(int? gateId, string? boardingGroup)
    {
        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException(
                "El GateId debe ser un entero positivo cuando se proporciona.", nameof(gateId));

        ValidateBoardingGroup(boardingGroup);

        GateId        = gateId;
        BoardingGroup = boardingGroup?.Trim();
    }

    private static void ValidateBoardingPassCode(string boardingPassCode)
    {
        if (string.IsNullOrWhiteSpace(boardingPassCode))
            throw new ArgumentException("El código del pase de abordar no puede estar vacío.", nameof(boardingPassCode));

        if (boardingPassCode.Trim().Length > 30)
            throw new ArgumentException(
                "El código del pase de abordar no puede exceder 30 caracteres.", nameof(boardingPassCode));
    }

    private static void ValidateBoardingGroup(string? boardingGroup)
    {
        if (boardingGroup is not null && boardingGroup.Trim().Length > 10)
            throw new ArgumentException(
                "El grupo de embarque no puede exceder 10 caracteres.", nameof(boardingGroup));
    }
}
