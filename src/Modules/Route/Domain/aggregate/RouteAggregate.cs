namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.ValueObject;












public sealed class RouteAggregate
{
    public RouteId   Id                   { get; private set; }
    public int       OriginAirportId      { get; private set; }
    public int       DestinationAirportId { get; private set; }
    public DateTime  CreatedAt            { get; private set; }

    private RouteAggregate()
    {
        Id = null!;
    }

    public RouteAggregate(
        RouteId  id,
        int      originAirportId,
        int      destinationAirportId,
        DateTime createdAt)
    {
        if (originAirportId <= 0)
            throw new ArgumentException(
                "OriginAirportId must be a positive integer.", nameof(originAirportId));

        if (destinationAirportId <= 0)
            throw new ArgumentException(
                "DestinationAirportId must be a positive integer.", nameof(destinationAirportId));

        if (originAirportId == destinationAirportId)
            throw new Exception("El aeropuerto de origen y el de destino deben ser distintos.");

        Id                   = id;
        OriginAirportId      = originAirportId;
        DestinationAirportId = destinationAirportId;
        CreatedAt            = createdAt;
    }
}
