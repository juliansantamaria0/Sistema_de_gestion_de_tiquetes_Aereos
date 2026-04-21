namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Infrastructure.Entity;

public sealed class RouteScheduleEntity
{
    public int      Id            { get; set; }
    public int      BaseFlightId  { get; set; }
    public byte     DayOfWeek     { get; set; }
    public TimeOnly DepartureTime { get; set; }
}
