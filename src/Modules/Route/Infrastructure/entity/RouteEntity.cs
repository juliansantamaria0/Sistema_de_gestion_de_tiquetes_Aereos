namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity;

public sealed class RouteEntity
{
    public int      Id                   { get; set; }
    public int      OriginAirportId      { get; set; }
    public int      DestinationAirportId { get; set; }
    public DateTime CreatedAt            { get; set; }
}
