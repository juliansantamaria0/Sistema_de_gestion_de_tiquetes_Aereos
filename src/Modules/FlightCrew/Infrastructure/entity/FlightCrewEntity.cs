namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Infrastructure.Entity;

public sealed class FlightCrewEntity
{
    public int      Id                { get; set; }
    public int      ScheduledFlightId { get; set; }
    public int      EmployeeId        { get; set; }
    public int      CrewRoleId        { get; set; }
    public DateTime CreatedAt         { get; set; }
}
