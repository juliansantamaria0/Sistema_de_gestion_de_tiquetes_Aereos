namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateRouteUseCase
{
    private readonly IRouteRepository _repository;
    private readonly IUnitOfWork      _unitOfWork;

    public CreateRouteUseCase(IRouteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RouteAggregate> ExecuteAsync(
        int               originAirportId,
        int               destinationAirportId,
        CancellationToken cancellationToken = default)
    {
        var route = new RouteAggregate(
            new RouteId(1), originAirportId, destinationAirportId, DateTime.UtcNow);

        await _repository.AddAsync(route, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return route;
    }
}
