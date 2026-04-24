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

    private ScheduledFlightAggregate(
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

    public static ScheduledFlightAggregate Create(
        int      baseFlightId,
        int      aircraftId,
        int?     gateId,
        DateOnly departureDate,
        TimeOnly departureTime,
        DateTime estimatedArrivalDatetime,
        int      flightStatusId)
    {
        if (baseFlightId <= 0)
            throw new ArgumentException("BaseFlightId must be a positive integer.", nameof(baseFlightId));

        if (aircraftId <= 0)
            throw new ArgumentException("AircraftId must be a positive integer.", nameof(aircraftId));

        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException("GateId must be a positive integer when provided.", nameof(gateId));

        if (flightStatusId <= 0)
            throw new ArgumentException("FlightStatusId must be a positive integer.", nameof(flightStatusId));

        // DepartureDate/DepartureTime are modeled as calendar date & clock time (no timezone).
        // Compare using local "today" to avoid false negatives when the machine is behind UTC.
        var todayLocal = DateOnly.FromDateTime(DateTime.Now);
        if (departureDate < todayLocal)
            throw new Exception("La fecha de salida programada no puede ser anterior al día actual.");

        ValidateArrivalAfterDeparture(departureDate, departureTime, estimatedArrivalDatetime);

        return new ScheduledFlightAggregate(
            new ScheduledFlightId(0),
            baseFlightId,
            aircraftId,
            gateId,
            departureDate,
            departureTime,
            estimatedArrivalDatetime,
            flightStatusId,
            DateTime.UtcNow);
    }

    public static ScheduledFlightAggregate Reconstitute(
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
        => new(
            id,
            baseFlightId,
            aircraftId,
            gateId,
            departureDate,
            departureTime,
            estimatedArrivalDatetime,
            flightStatusId,
            createdAt,
            updatedAt);

    
    
    
    
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

        var todayLocal = DateOnly.FromDateTime(DateTime.Now);
        if (departureDate < todayLocal)
            throw new Exception("La fecha de salida programada no puede ser anterior al día actual.");

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
        
        // Keep DateTimeKind.Unspecified to match values coming from the database.
        var departureDateTime = departureDate.ToDateTime(departureTime);

        if (estimatedArrival <= departureDateTime)
            throw new ArgumentException(
                "EstimatedArrivalDatetime must be after the departure datetime.",
                nameof(estimatedArrival));
    }
}
