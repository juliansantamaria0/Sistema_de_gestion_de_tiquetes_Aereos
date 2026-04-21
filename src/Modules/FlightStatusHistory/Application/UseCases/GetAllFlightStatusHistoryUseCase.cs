namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Repositories;

public sealed class GetAllFlightStatusHistoryUseCase
{
    private readonly IFlightStatusHistoryRepository _repository;

    public GetAllFlightStatusHistoryUseCase(IFlightStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightStatusHistoryAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
