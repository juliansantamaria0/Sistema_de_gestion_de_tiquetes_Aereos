namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.ValueObject;

public interface IRouteScheduleRepository
{
    Task<RouteScheduleAggregate?>             GetByIdAsync(RouteScheduleId id,                   CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteScheduleAggregate>> GetAllAsync(                                        CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteScheduleAggregate>> GetByBaseFlightAsync(int baseFlightId,              CancellationToken cancellationToken = default);
    Task                                      AddAsync(RouteScheduleAggregate routeSchedule,      CancellationToken cancellationToken = default);
    Task                                      UpdateAsync(RouteScheduleAggregate routeSchedule,   CancellationToken cancellationToken = default);
    Task                                      DeleteAsync(RouteScheduleId id,                     CancellationToken cancellationToken = default);
}
