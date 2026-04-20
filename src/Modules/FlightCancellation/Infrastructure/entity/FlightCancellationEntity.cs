namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Infrastructure.Entity;

public sealed class FlightCancellationEntity
{
    public int      Id                   { get; set; }
    public int      ScheduledFlightId    { get; set; }
    public int      CancellationReasonId { get; set; }
    public DateTime CancelledAt          { get; set; }
    public string?  Notes                { get; set; }
}
