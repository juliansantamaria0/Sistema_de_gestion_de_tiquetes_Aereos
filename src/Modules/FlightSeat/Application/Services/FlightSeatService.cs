namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;

public sealed class FlightSeatService : IFlightSeatService
{
    private readonly CreateFlightSeatUseCase                  _create;
    private readonly DeleteFlightSeatUseCase                  _delete;
    private readonly GetAllFlightSeatsUseCase                 _getAll;
    private readonly GetFlightSeatByIdUseCase                 _getById;
    private readonly ChangeFlightSeatStatusUseCase            _changeStatus;
    private readonly GetFlightSeatsByFlightUseCase            _getByFlight;
    private readonly GetAvailableFlightSeatsByFlightUseCase   _getAvailable;

    public FlightSeatService(
        CreateFlightSeatUseCase                create,
        DeleteFlightSeatUseCase                delete,
        GetAllFlightSeatsUseCase               getAll,
        GetFlightSeatByIdUseCase               getById,
        ChangeFlightSeatStatusUseCase          changeStatus,
        GetFlightSeatsByFlightUseCase          getByFlight,
        GetAvailableFlightSeatsByFlightUseCase getAvailable)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _changeStatus = changeStatus;
        _getByFlight  = getByFlight;
        _getAvailable = getAvailable;
    }

    public async Task<FlightSeatDto> CreateAsync(
        int               scheduledFlightId,
        int               seatMapId,
        int               seatStatusId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(scheduledFlightId, seatMapId, seatStatusId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightSeatDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FlightSeatDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task ChangeStatusAsync(
        int               id,
        int               seatStatusId,
        CancellationToken cancellationToken = default)
        => await _changeStatus.ExecuteAsync(id, seatStatusId, cancellationToken);

    public async Task<IEnumerable<FlightSeatDto>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<IEnumerable<FlightSeatDto>> GetAvailableByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getAvailable.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    

    private static FlightSeatDto ToDto(FlightSeatAggregate agg)
        => new(agg.Id.Value, agg.ScheduledFlightId, agg.SeatMapId,
               agg.SeatStatusId, agg.CreatedAt, agg.UpdatedAt);
}
