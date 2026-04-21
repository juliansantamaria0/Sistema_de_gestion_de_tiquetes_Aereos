namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity;

public sealed class ScheduledFlightEntity
{
    public int       Id                       { get; set; }
    public int       BaseFlightId             { get; set; }
    public int       AircraftId               { get; set; }
    public int?      GateId                   { get; set; }
    public DateOnly  DepartureDate            { get; set; }
    public TimeOnly  DepartureTime            { get; set; }
    public DateTime  EstimatedArrivalDatetime { get; set; }
    public int       FlightStatusId           { get; set; }
    public DateTime  CreatedAt                { get; set; }
    public DateTime? UpdatedAt                { get; set; }
}
