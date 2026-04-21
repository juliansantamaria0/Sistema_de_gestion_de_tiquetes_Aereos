namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;

public sealed class FlightStatusHistoryService : IFlightStatusHistoryService
{
    private readonly RecordFlightStatusHistoryUseCase        _record;
    private readonly DeleteFlightStatusHistoryUseCase        _delete;
    private readonly GetAllFlightStatusHistoryUseCase        _getAll;
    private readonly GetFlightStatusHistoryByIdUseCase       _getById;
    private readonly GetFlightStatusHistoryByFlightUseCase   _getByFlight;

    public FlightStatusHistoryService(
        RecordFlightStatusHistoryUseCase      record,
        DeleteFlightStatusHistoryUseCase      delete,
        GetAllFlightStatusHistoryUseCase      getAll,
        GetFlightStatusHistoryByIdUseCase     getById,
        GetFlightStatusHistoryByFlightUseCase getByFlight)
    {
        _record      = record;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _getByFlight = getByFlight;
    }

    public async Task<FlightStatusHistoryDto> RecordAsync(
        int               scheduledFlightId,
        int               flightStatusId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var agg = await _record.ExecuteAsync(
            scheduledFlightId, flightStatusId, notes, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightStatusHistoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FlightStatusHistoryDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task<IEnumerable<FlightStatusHistoryDto>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    private static FlightStatusHistoryDto ToDto(FlightStatusHistoryAggregate agg)
        => new(agg.Id.Value, agg.ScheduledFlightId, agg.FlightStatusId, agg.ChangedAt, agg.Notes);
}
