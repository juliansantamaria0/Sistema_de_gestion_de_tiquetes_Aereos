namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Repositories;

/// <summary>Obtiene todas las rutas que parten de un aeropuerto origen.</summary>
public sealed class GetRoutesByOriginUseCase
{
    private readonly IRouteRepository _repository;

    public GetRoutesByOriginUseCase(IRouteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RouteAggregate>> ExecuteAsync(
        int               originAirportId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByOriginAsync(originAirportId, cancellationToken);
}
