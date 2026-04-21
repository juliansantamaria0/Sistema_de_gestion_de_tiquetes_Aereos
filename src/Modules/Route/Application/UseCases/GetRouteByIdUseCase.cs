namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.ValueObject;

public sealed class GetRouteByIdUseCase
{
    private readonly IRouteRepository _repository;

    public GetRouteByIdUseCase(IRouteRepository repository)
    {
        _repository = repository;
    }

    public async Task<RouteAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new RouteId(id), cancellationToken);
}
