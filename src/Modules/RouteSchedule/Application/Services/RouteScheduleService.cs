namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;

public sealed class RouteScheduleService : IRouteScheduleService
{
    private readonly CreateRouteScheduleUseCase             _create;
    private readonly DeleteRouteScheduleUseCase             _delete;
    private readonly GetAllRouteSchedulesUseCase            _getAll;
    private readonly GetRouteScheduleByIdUseCase            _getById;
    private readonly UpdateRouteScheduleUseCase             _update;
    private readonly GetRouteSchedulesByBaseFlightUseCase   _getByBaseFlight;

    public RouteScheduleService(
        CreateRouteScheduleUseCase           create,
        DeleteRouteScheduleUseCase           delete,
        GetAllRouteSchedulesUseCase          getAll,
        GetRouteScheduleByIdUseCase          getById,
        UpdateRouteScheduleUseCase           update,
        GetRouteSchedulesByBaseFlightUseCase getByBaseFlight)
    {
        _create          = create;
        _delete          = delete;
        _getAll          = getAll;
        _getById         = getById;
        _update          = update;
        _getByBaseFlight = getByBaseFlight;
    }

    public async Task<RouteScheduleDto> CreateAsync(
        int               baseFlightId,
        byte              dayOfWeek,
        TimeOnly          departureTime,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(baseFlightId, dayOfWeek, departureTime, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<RouteScheduleDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<RouteScheduleDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        byte              dayOfWeek,
        TimeOnly          departureTime,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, dayOfWeek, departureTime, cancellationToken);

    public async Task<IEnumerable<RouteScheduleDto>> GetByBaseFlightAsync(
        int               baseFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByBaseFlight.ExecuteAsync(baseFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    // ── Mapper privado ────────────────────────────────────────────────────────

    private static RouteScheduleDto ToDto(RouteScheduleAggregate agg)
        => new(agg.Id.Value, agg.BaseFlightId, agg.DayOfWeek, agg.DayOfWeekName, agg.DepartureTime);
}
