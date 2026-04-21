namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Infrastructure.Entity;

public sealed class FlightStatusHistoryEntity
{
    public int      Id                { get; set; }
    public int      ScheduledFlightId { get; set; }
    public int      FlightStatusId    { get; set; }
    public DateTime ChangedAt         { get; set; }
    public string?  Notes             { get; set; }
}
