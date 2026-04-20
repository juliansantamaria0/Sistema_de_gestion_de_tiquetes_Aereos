namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;

public sealed class GetAllFlightDelaysUseCase
{
    private readonly IFlightDelayRepository _repository;

    public GetAllFlightDelaysUseCase(IFlightDelayRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightDelayAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
