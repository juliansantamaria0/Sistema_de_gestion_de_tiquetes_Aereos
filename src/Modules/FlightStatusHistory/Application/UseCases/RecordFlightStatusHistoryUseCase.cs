namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class RecordFlightStatusHistoryUseCase
{
    private readonly IFlightStatusHistoryRepository _repository;
    private readonly IUnitOfWork                    _unitOfWork;

    public RecordFlightStatusHistoryUseCase(
        IFlightStatusHistoryRepository repository,
        IUnitOfWork                    unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightStatusHistoryAggregate> ExecuteAsync(
        int               scheduledFlightId,
        int               flightStatusId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var entry = new FlightStatusHistoryAggregate(
            new FlightStatusHistoryId(await GetNextIdAsync(cancellationToken)),
            scheduledFlightId, flightStatusId, DateTime.UtcNow, notes);

        await _repository.AddAsync(entry, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return entry;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
