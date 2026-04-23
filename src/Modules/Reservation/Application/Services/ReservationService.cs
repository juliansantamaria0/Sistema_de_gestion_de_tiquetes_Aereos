namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;

public sealed class ReservationService : IReservationService
{
    private readonly CreateReservationUseCase         _create;
    private readonly DeleteReservationUseCase         _delete;
    private readonly GetAllReservationsUseCase        _getAll;
    private readonly GetReservationByIdUseCase        _getById;
    private readonly UpdateReservationUseCase         _update;
    private readonly ConfirmReservationUseCase        _confirm;
    private readonly CancelReservationUseCase         _cancel;
    private readonly GetReservationsByCustomerUseCase _getByCustomer;
    private readonly GetReservationsByFlightUseCase   _getByFlight;

    public ReservationService(
        CreateReservationUseCase         create,
        DeleteReservationUseCase         delete,
        GetAllReservationsUseCase        getAll,
        GetReservationByIdUseCase        getById,
        UpdateReservationUseCase         update,
        ConfirmReservationUseCase        confirm,
        CancelReservationUseCase         cancel,
        GetReservationsByCustomerUseCase getByCustomer,
        GetReservationsByFlightUseCase   getByFlight)
    {
        _create        = create;
        _delete        = delete;
        _getAll        = getAll;
        _getById       = getById;
        _update        = update;
        _confirm       = confirm;
        _cancel        = cancel;
        _getByCustomer = getByCustomer;
        _getByFlight   = getByFlight;
    }

    public async Task<ReservationDto> CreateAsync(
        string            code,
        int               customerId,
        int               scheduledFlightId,
        int               reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(code, customerId, scheduledFlightId, reservationStatusId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<ReservationDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<ReservationDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task ChangeStatusAsync(
        int               id,
        int               reservationStatusId,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, reservationStatusId, cancellationToken);

    public async Task ConfirmAsync(
        int               id,
        int               confirmedReservationStatusId,
        CancellationToken cancellationToken = default)
        => await _confirm.ExecuteAsync(id, confirmedReservationStatusId, cancellationToken);

    public async Task CancelAsync(
        int               id,
        int               cancelledReservationStatusId,
        CancellationToken cancellationToken = default)
        => await _cancel.ExecuteAsync(id, cancelledReservationStatusId, cancellationToken);

    public async Task<IEnumerable<ReservationDto>> GetByCustomerAsync(
        int               customerId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByCustomer.ExecuteAsync(customerId, cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<IEnumerable<ReservationDto>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    

    private static ReservationDto ToDto(ReservationAggregate agg)
        => new(
            agg.Id.Value,
            agg.ReservationCode,
            agg.CustomerId,
            agg.ScheduledFlightId,
            agg.ReservationDate,
            agg.ReservationStatusId,
            agg.ConfirmedAt,
            agg.CancelledAt,
            agg.CreatedAt,
            agg.UpdatedAt);
}
