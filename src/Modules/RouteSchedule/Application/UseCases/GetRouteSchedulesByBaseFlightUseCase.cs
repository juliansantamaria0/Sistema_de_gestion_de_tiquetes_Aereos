namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;






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
