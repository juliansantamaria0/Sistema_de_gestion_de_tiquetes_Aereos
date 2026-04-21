namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.Interfaces;

public interface IRouteService
{
    Task<RouteDto?>             GetByIdAsync(int id,                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetAllAsync(                                               CancellationToken cancellationToken = default);
    Task<RouteDto?>             GetByAirportsAsync(int originId, int destinationId,        CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetByOriginAsync(int originAirportId,                     CancellationToken cancellationToken = default);
    Task<RouteDto>              CreateAsync(int originAirportId, int destinationAirportId, CancellationToken cancellationToken = default);
    Task                        DeleteAsync(int id,                                        CancellationToken cancellationToken = default);
}

public sealed record RouteDto(
    int      Id,
    int      OriginAirportId,
    int      DestinationAirportId,
    DateTime CreatedAt);
