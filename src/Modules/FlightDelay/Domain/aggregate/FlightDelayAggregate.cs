namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;












public sealed class FlightDelayAggregate
{
    public FlightDelayId Id                { get; private set; }
    public int           ScheduledFlightId { get; private set; }
    public int           DelayReasonId     { get; private set; }
    public int           DelayMinutes      { get; private set; }
    public DateTime      ReportedAt        { get; private set; }

    private FlightDelayAggregate()
    {
        Id = null!;
    }

    public FlightDelayAggregate(
        FlightDelayId id,
        int           scheduledFlightId,
        int           delayReasonId,
        int           delayMinutes,
        DateTime      reportedAt)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (delayReasonId <= 0)
            throw new ArgumentException(
                "DelayReasonId must be a positive integer.", nameof(delayReasonId));

        ValidateDelayMinutes(delayMinutes);

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        DelayReasonId     = delayReasonId;
        DelayMinutes      = delayMinutes;
        ReportedAt        = reportedAt;
    }

    
    
    
    
    public void AdjustDelay(int delayMinutes)
    {
        ValidateDelayMinutes(delayMinutes);
        DelayMinutes = delayMinutes;
    }

    

    private static void ValidateDelayMinutes(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentException(
                "DelayMinutes must be greater than 0.", nameof(minutes));
    }
}
