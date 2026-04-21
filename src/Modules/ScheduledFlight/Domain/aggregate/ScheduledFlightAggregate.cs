namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;

/// <summary>
/// Instancia concreta de un vuelo programado para una fecha específica.
/// SQL: scheduled_flight.
///
/// Tipos .NET 8:
///   - departure_date  → DateOnly  (DATE SQL)
///   - departure_time  → TimeOnly  (TIME SQL)
///   - estimated_arrival_datetime → DateTime (DATETIME SQL — soporta vuelos nocturnos)
///
/// Invariantes:
///   - estimated_arrival_datetime debe ser posterior al momento de salida combinado.
///   - gate_id es nullable (puede asignarse después de crear el vuelo).
///   - UNIQUE (base_flight_id, departure_date, departure_time).
/// </summary>
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

    /// <summary>
    /// Actualiza los datos operativos del vuelo programado.
    /// base_flight_id no se permite cambiar (cambiaría la identidad del vuelo).
    /// </summary>
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

    /// <summary>Asigna o cambia la puerta de embarque sin tocar otros campos.</summary>
    public void AssignGate(int? gateId)
    {
        if (gateId.HasValue && gateId.Value <= 0)
            throw new ArgumentException("GateId must be a positive integer when provided.", nameof(gateId));

        GateId    = gateId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Cambia el estado del vuelo (SCHEDULED → ACTIVE → COMPLETED, etc.).</summary>
    public void ChangeStatus(int flightStatusId)
    {
        if (flightStatusId <= 0)
            throw new ArgumentException("FlightStatusId must be a positive integer.", nameof(flightStatusId));

        FlightStatusId = flightStatusId;
        UpdatedAt      = DateTime.UtcNow;
    }

    // ── Helpers privados ──────────────────────────────────────────────────────

    private static void ValidateArrivalAfterDeparture(
        DateOnly departureDate,
        TimeOnly departureTime,
        DateTime estimatedArrival)
    {
        // Combina DateOnly + TimeOnly para construir el DateTime de salida (UTC).
        var departureDateTime = departureDate.ToDateTime(departureTime, DateTimeKind.Utc);

        if (estimatedArrival <= departureDateTime)
            throw new ArgumentException(
                "EstimatedArrivalDatetime must be after the departure datetime.",
                nameof(estimatedArrival));
    }
}
