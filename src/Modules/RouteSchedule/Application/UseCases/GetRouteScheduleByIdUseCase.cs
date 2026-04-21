namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.ValueObject;

public sealed class GetRouteScheduleByIdUseCase
{
    private readonly IRouteScheduleRepository _repository;

    public GetRouteScheduleByIdUseCase(IRouteScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<RouteScheduleAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new RouteScheduleId(id), cancellationToken);
}
