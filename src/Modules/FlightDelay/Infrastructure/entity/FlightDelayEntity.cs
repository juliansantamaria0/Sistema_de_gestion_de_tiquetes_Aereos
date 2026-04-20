namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Entity;

public sealed class FlightDelayEntity
{
    public int      Id                { get; set; }
    public int      ScheduledFlightId { get; set; }
    public int      DelayReasonId     { get; set; }
    public int      DelayMinutes      { get; set; }
    public DateTime ReportedAt        { get; set; }
}
