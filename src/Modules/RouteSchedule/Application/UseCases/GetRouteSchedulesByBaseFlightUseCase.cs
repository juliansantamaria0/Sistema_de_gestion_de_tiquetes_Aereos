namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;

/// <summary>
/// Caso de uso adicional: obtiene todos los horarios de un vuelo base dado.
/// Útil para consultar en qué días opera un vuelo antes de generar
/// instancias de scheduled_flight.
/// </summary>
public sealed class GetRouteSchedulesByBaseFlightUseCase
{
    private readonly IRouteScheduleRepository _repository;

    public GetRouteSchedulesByBaseFlightUseCase(IRouteScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RouteScheduleAggregate>> ExecuteAsync(
        int               baseFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByBaseFlightAsync(baseFlightId, cancellationToken);
}
