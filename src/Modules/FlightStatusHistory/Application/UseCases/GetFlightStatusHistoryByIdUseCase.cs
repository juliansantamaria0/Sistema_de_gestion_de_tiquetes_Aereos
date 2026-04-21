namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.ValueObject;

public sealed class GetFlightStatusHistoryByIdUseCase
{
    private readonly IFlightStatusHistoryRepository _repository;

    public GetFlightStatusHistoryByIdUseCase(IFlightStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightStatusHistoryAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightStatusHistoryId(id), cancellationToken);
}
