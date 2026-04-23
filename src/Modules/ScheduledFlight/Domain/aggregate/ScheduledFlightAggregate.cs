namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;















public sealed class ScheduledFlightAggregate
{
    public ScheduledFlightId Id                       { get; private set; }
    public int               BaseFlightId             { get; private set; }
    public int               AircraftId               { get; private set; }
    public int?              GateId                   { get; private set; }
    public DateOnly          DepartureDate            { get; private set; }
    public TimeOnly          DepartureTime            { get; private set; }
    public DateTime          EstimatedArrivalDatetime { get; private set; }
    public int               FlightStatusId           { get; private set; }
    public DateTime          CreatedAt                { get; private set; }
    public DateTime?         UpdatedAt                { get; private set; }

    private ScheduledFlightAggregate()
    {
        Id = null!;
    }

    public ScheduledFlightAggregate(
        ScheduledFlightId id,
        int               baseFlightId,
        int               aircraftId,
        int?              gateId,
        DateOnly          departureDate,
        TimeOnly          departureTime,
        DateTime          estimatedArrivalDatetime,
        int               flightStatusId,
        DateTime          createdAt,
        DateTime?         updatedAt = null)
    {
        if (baseFlightId <= 0)
            throw new ArgumentException("BaseFlightId must be a positive integer.", nameof(baseFlightId));

        if (aircraftId <= 0)
            throw new ArgumentException("AircraftId must be a positive integer.", nameof(aircraftId));

        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException("GateId must be a positive integer when provided.", nameof(gateId));

        if (flightStatusId <= 0)
            throw new ArgumentException("FlightStatusId must be a positive integer.", nameof(flightStatusId));

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);
        if (departureDate < todayUtc)
            throw new Exception("La fecha de salida programada no puede ser anterior al día actual (UTC).");

        ValidateArrivalAfterDeparture(departureDate, departureTime, estimatedArrivalDatetime);

        Id                       = id;
        BaseFlightId             = baseFlightId;
        AircraftId               = aircraftId;
        GateId                   = gateId;
        DepartureDate            = departureDate;
        DepartureTime            = departureTime;
        EstimatedArrivalDatetime = estimatedArrivalDatetime;
        FlightStatusId           = flightStatusId;
        CreatedAt                = createdAt;
        UpdatedAt                = updatedAt;
    }

    
    
    
    
    public void Update(
        int      aircraftId,
        int?     gateId,
        DateOnly departureDate,
        TimeOnly departureTime,
        DateTime estimatedArrivalDatetime,
        int      flightStatusId)
    {
        if (aircraftId <= 0)
            throw new ArgumentException("AircraftId must be a positive integer.", nameof(aircraftId));

        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException("GateId must be a positive integer when provided.", nameof(gateId));

        if (flightStatusId <= 0)
            throw new ArgumentException("FlightStatusId must be a positive integer.", nameof(flightStatusId));

        ValidateArrivalAfterDeparture(departureDate, departureTime, estimatedArrivalDatetime);

        AircraftId               = aircraftId;
        GateId                   = gateId;
        DepartureDate            = departureDate;
        DepartureTime            = departureTime;
        EstimatedArrivalDatetime = estimatedArrivalDatetime;
        FlightStatusId           = flightStatusId;
        UpdatedAt                = DateTime.UtcNow;
    }

    
    public void AssignGate(int? gateId)
    {
        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException("GateId must be a positive integer when provided.", nameof(gateId));

        GateId    = gateId;
        UpdatedAt = DateTime.UtcNow;
    }

    
    public void ChangeStatus(int flightStatusId)
    {
        if (flightStatusId <= 0)
            throw new ArgumentException("FlightStatusId must be a positive integer.", nameof(flightStatusId));

        FlightStatusId = flightStatusId;
        UpdatedAt      = DateTime.UtcNow;
    }

    

    private static void ValidateArrivalAfterDeparture(
        DateOnly departureDate,
        TimeOnly departureTime,
        DateTime estimatedArrival)
    {
        
        var departureDateTime = departureDate.ToDateTime(departureTime, DateTimeKind.Utc);

        if (estimatedArrival <= departureDateTime)
            throw new ArgumentException(
                "EstimatedArrivalDatetime must be after the departure datetime.",
                nameof(estimatedArrival));
    }
}
