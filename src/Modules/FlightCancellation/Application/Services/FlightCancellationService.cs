namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;

public sealed class FlightCancellationService : IFlightCancellationService
{
    private readonly CreateFlightCancellationUseCase         _create;
    private readonly DeleteFlightCancellationUseCase         _delete;
    private readonly GetAllFlightCancellationsUseCase        _getAll;
    private readonly GetFlightCancellationByIdUseCase        _getById;
    private readonly UpdateFlightCancellationUseCase         _update;
    private readonly GetFlightCancellationByFlightUseCase    _getByFlight;

    public FlightCancellationService(
        CreateFlightCancellationUseCase      create,
        DeleteFlightCancellationUseCase      delete,
        GetAllFlightCancellationsUseCase     getAll,
        GetFlightCancellationByIdUseCase     getById,
        UpdateFlightCancellationUseCase      update,
        GetFlightCancellationByFlightUseCase getByFlight)
    {
        _create      = create;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _update      = update;
        _getByFlight = getByFlight;
    }

    public async Task<FlightCancellationDto> CreateAsync(
        int               scheduledFlightId,
        int               cancellationReasonId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            scheduledFlightId, cancellationReasonId, notes, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightCancellationDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FlightCancellationDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateNotesAsync(
        int               id,
        string?           notes,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, notes, cancellationToken);

    public async Task<FlightCancellationDto?> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    

    private static FlightCancellationDto ToDto(FlightCancellationAggregate agg)
        => new(
            agg.Id.Value,
            agg.ScheduledFlightId,
            agg.CancellationReasonId,
            agg.CancelledAt,
            agg.Notes);
}
