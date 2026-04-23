namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.ValueObject;

public interface IRouteRepository
{
    Task<RouteAggregate?>             GetByIdAsync(RouteId id,                                         CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetAllAsync(                                                     CancellationToken cancellationToken = default);
    Task<RouteAggregate?>             GetByAirportsAsync(int originId, int destinationId,              CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetByOriginAsync(int originAirportId,                           CancellationToken cancellationToken = default);
    Task                              AddAsync(RouteAggregate route,                                   CancellationToken cancellationToken = default);
    Task                              DeleteAsync(RouteId id,                                          CancellationToken cancellationToken = default);
    
}
