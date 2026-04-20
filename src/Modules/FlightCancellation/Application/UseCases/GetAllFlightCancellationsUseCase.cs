namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;

public sealed class GetAllFlightCancellationsUseCase
{
    private readonly IFlightCancellationRepository _repository;

    public GetAllFlightCancellationsUseCase(IFlightCancellationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightCancellationAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
