namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Repositories;

public sealed class GetFlightStatusHistoryByFlightUseCase
{
    private readonly IFlightStatusHistoryRepository _repository;

    public GetFlightStatusHistoryByFlightUseCase(IFlightStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightStatusHistoryAggregate>> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
