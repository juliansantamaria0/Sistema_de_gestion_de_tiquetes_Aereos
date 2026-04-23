namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;

public sealed class ScheduledFlightService : IScheduledFlightService
{
    private readonly CreateScheduledFlightUseCase             _create;
    private readonly DeleteScheduledFlightUseCase             _delete;
    private readonly GetAllScheduledFlightsUseCase            _getAll;
    private readonly GetScheduledFlightByIdUseCase            _getById;
    private readonly UpdateScheduledFlightUseCase             _update;
    private readonly GetScheduledFlightsByBaseFlightUseCase   _getByBaseFlight;
    private readonly GetScheduledFlightsByDateUseCase         _getByDate;

    public ScheduledFlightService(
        CreateScheduledFlightUseCase           create,
        DeleteScheduledFlightUseCase           delete,
        GetAllScheduledFlightsUseCase          getAll,
        GetScheduledFlightByIdUseCase          getById,
        UpdateScheduledFlightUseCase           update,
        GetScheduledFlightsByBaseFlightUseCase getByBaseFlight,
        GetScheduledFlightsByDateUseCase       getByDate)
    {
        _create          = create;
        _delete          = delete;
        _getAll          = getAll;
        _getById         = getById;
        _update          = update;
        _getByBaseFlight = getByBaseFlight;
        _getByDate       = getByDate;
    }

    public async Task<ScheduledFlightDto> CreateAsync(
        CreateScheduledFlightRequest request,
        CancellationToken            cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(request, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<ScheduledFlightDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<ScheduledFlightDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int                          id,
        UpdateScheduledFlightRequest request,
        CancellationToken            cancellationToken = default)
        => await _update.ExecuteAsync(id, request, cancellationToken);

    public async Task<IEnumerable<ScheduledFlightDto>> GetByBaseFlightAsync(
        int               baseFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByBaseFlight.ExecuteAsync(baseFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<IEnumerable<ScheduledFlightDto>> GetByDateAsync(
        DateOnly          date,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByDate.ExecuteAsync(date, cancellationToken);
        return list.Select(ToDto);
    }

    

    private static ScheduledFlightDto ToDto(ScheduledFlightAggregate agg)
        => new(
            agg.Id.Value,
            agg.BaseFlightId,
            agg.AircraftId,
            agg.GateId,
            agg.DepartureDate,
            agg.DepartureTime,
            agg.EstimatedArrivalDatetime,
            agg.FlightStatusId,
            agg.CreatedAt,
            agg.UpdatedAt);
}
