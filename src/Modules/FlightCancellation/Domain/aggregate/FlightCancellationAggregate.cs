namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;














public sealed class FlightCancellationAggregate
{
    public FlightCancellationId Id                    { get; private set; }
    public int                  ScheduledFlightId     { get; private set; }
    public int                  CancellationReasonId  { get; private set; }
    public DateTime             CancelledAt           { get; private set; }
    public string?              Notes                 { get; private set; }

    private FlightCancellationAggregate()
    {
        Id = null!;
    }

    public FlightCancellationAggregate(
        FlightCancellationId id,
        int                  scheduledFlightId,
        int                  cancellationReasonId,
        DateTime             cancelledAt,
        string?              notes = null)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (cancellationReasonId <= 0)
            throw new ArgumentException(
                "CancellationReasonId must be a positive integer.", nameof(cancellationReasonId));

        ValidateNotes(notes);

        Id                   = id;
        ScheduledFlightId    = scheduledFlightId;
        CancellationReasonId = cancellationReasonId;
        CancelledAt          = cancelledAt;
        Notes                = notes?.Trim();
    }

    
    
    
    
    public void UpdateNotes(string? notes)
    {
        ValidateNotes(notes);
        Notes = notes?.Trim();
    }

    

    private static void ValidateNotes(string? notes)
    {
        if (notes is not null && notes.Trim().Length > 250)
            throw new ArgumentException(
                "Notes cannot exceed 250 characters.", nameof(notes));
    }
}
