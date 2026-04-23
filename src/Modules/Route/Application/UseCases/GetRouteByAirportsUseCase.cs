namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Repositories;






public sealed class GetRouteByAirportsUseCase
{
    private readonly IRouteRepository _repository;

    public GetRouteByAirportsUseCase(IRouteRepository repository)
    {
        _repository = repository;
    }

    public async Task<RouteAggregate?> ExecuteAsync(
        int               originId,
        int               destinationId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByAirportsAsync(originId, destinationId, cancellationToken);
}
