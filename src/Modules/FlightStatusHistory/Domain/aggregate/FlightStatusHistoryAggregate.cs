namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.ValueObject;








public sealed class FlightStatusHistoryAggregate
{
    public FlightStatusHistoryId Id                { get; private set; }
    public int                   ScheduledFlightId { get; private set; }
    public int                   FlightStatusId    { get; private set; }
    public DateTime              ChangedAt         { get; private set; }
    public string?               Notes             { get; private set; }

    private FlightStatusHistoryAggregate() { Id = null!; }

    public FlightStatusHistoryAggregate(
        FlightStatusHistoryId id,
        int                   scheduledFlightId,
        int                   flightStatusId,
        DateTime              changedAt,
        string?               notes = null)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (flightStatusId <= 0)
            throw new ArgumentException(
                "FlightStatusId must be a positive integer.", nameof(flightStatusId));

        if (notes is not null && notes.Trim().Length > 250)
            throw new ArgumentException(
                "Notes cannot exceed 250 characters.", nameof(notes));

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        FlightStatusId    = flightStatusId;
        ChangedAt         = changedAt;
        Notes             = notes?.Trim();
    }
}
