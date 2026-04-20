namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;

public sealed class FlightDelayService : IFlightDelayService
{
    private readonly CreateFlightDelayUseCase         _create;
    private readonly DeleteFlightDelayUseCase         _delete;
    private readonly GetAllFlightDelaysUseCase        _getAll;
    private readonly GetFlightDelayByIdUseCase        _getById;
    private readonly UpdateFlightDelayUseCase         _update;
    private readonly GetFlightDelaysByFlightUseCase   _getByFlight;

    public FlightDelayService(
        CreateFlightDelayUseCase       create,
        DeleteFlightDelayUseCase       delete,
        GetAllFlightDelaysUseCase      getAll,
        GetFlightDelayByIdUseCase      getById,
        UpdateFlightDelayUseCase       update,
        GetFlightDelaysByFlightUseCase getByFlight)
    {
        _create      = create;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _update      = update;
        _getByFlight = getByFlight;
    }

    public async Task<FlightDelayDto> CreateAsync(
        int               scheduledFlightId,
        int               delayReasonId,
        int               delayMinutes,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            scheduledFlightId, delayReasonId, delayMinutes, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightDelayDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FlightDelayDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task AdjustDelayAsync(
        int               id,
        int               delayMinutes,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, delayMinutes, cancellationToken);

    public async Task<IEnumerable<FlightDelayDto>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    // ── Mapper privado ────────────────────────────────────────────────────────

    private static FlightDelayDto ToDto(FlightDelayAggregate agg)
        => new(
            agg.Id.Value,
            agg.ScheduledFlightId,
            agg.DelayReasonId,
            agg.DelayMinutes,
            agg.ReportedAt);
}
