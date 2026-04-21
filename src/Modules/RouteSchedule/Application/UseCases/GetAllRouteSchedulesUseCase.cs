namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;

public sealed class GetAllRouteSchedulesUseCase
{
    private readonly IRouteScheduleRepository _repository;

    public GetAllRouteSchedulesUseCase(IRouteScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RouteScheduleAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
