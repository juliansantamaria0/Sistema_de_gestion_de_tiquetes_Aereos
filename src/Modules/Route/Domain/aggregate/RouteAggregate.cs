namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.ValueObject;

/// <summary>
/// Ruta entre dos aeropuertos distintos.
/// SQL: route.
///
/// Invariante (espejo de chk_route_different):
///   origin_airport_id ≠ destination_airport_id.
/// UNIQUE: (origin_airport_id, destination_airport_id).
///
/// Una ruta es inmutable tras su creación — los aeropuertos no cambian.
/// Si se necesita una ruta diferente, se crea una nueva.
/// </summary>
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

        // Espejo de chk_route_different
        if (originAirportId == destinationAirportId)
            throw new ArgumentException(
                "Origin and destination airports must be different. [chk_route_different]");

        Id                   = id;
        OriginAirportId      = originAirportId;
        DestinationAirportId = destinationAirportId;
        CreatedAt            = createdAt;
    }
}
