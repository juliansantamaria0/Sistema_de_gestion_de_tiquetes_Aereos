namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class ReservationService : IReservationService
{
    private readonly CreateReservationUseCase         _create;
    private readonly DeleteReservationUseCase         _delete;
    private readonly GetAllReservationsUseCase        _getAll;
    private readonly GetReservationByIdUseCase        _getById;
    private readonly UpdateReservationUseCase         _update;
    private readonly ConfirmReservationUseCase        _confirm;
    private readonly CancelReservationUseCase         _cancel;
    private readonly ReprogramarReservaUseCase         _reprogramar;
    private readonly CrearSolicitudListaEsperaUseCase    _crearListaEspera;
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
        ReprogramarReservaUseCase        reprogramar,
        CrearSolicitudListaEsperaUseCase crearListaEspera,
        GetReservationsByCustomerUseCase getByCustomer,
        GetReservationsByFlightUseCase   getByFlight)
    {
        _create         = create;
        _delete         = delete;
        _getAll         = getAll;
        _getById        = getById;
        _update         = update;
        _confirm        = confirm;
        _cancel         = cancel;
        _reprogramar    = reprogramar;
        _crearListaEspera = crearListaEspera;
        _getByCustomer  = getByCustomer;
        _getByFlight    = getByFlight;
    }

    public async Task<ReservationDto> CreateAsync(
        int               customerId,
        int               scheduledFlightId,
        int               reservationStatusId,
        bool              requireAvailableSeats = true,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            customerId, scheduledFlightId, reservationStatusId, requireAvailableSeats, cancellationToken);
        return ToDto(agg);
    }

    public async Task<ReservationDto> CreateForCurrentUserAsync(
        int               scheduledFlightId,
        int               reservationStatusId,
        bool              requireAvailableSeats = true,
        CancellationToken cancellationToken = default)
    {
        if (!CurrentUser.IsAuthenticated || !CurrentUser.CustomerId.HasValue)
            throw new InvalidOperationException("Debe iniciar sesión para realizar una reserva.");

        var agg = await _create.ExecuteAsync(
            CurrentUser.CustomerId.Value, scheduledFlightId, reservationStatusId, requireAvailableSeats, cancellationToken);
        return ToDto(agg);
    }

    public async Task<IReadOnlyList<ReservationDto>> CrearSolicitudListaEsperaAsync(
        int scheduledFlightId,
        IReadOnlyList<(int PassengerId, int FareTypeId)> puestos,
        CancellationToken cancellationToken = default)
    {
        if (!CurrentUser.IsAuthenticated || !CurrentUser.CustomerId.HasValue)
            throw new InvalidOperationException("Debe iniciar sesión.");
        var list = await _crearListaEspera.ExecuteAsync(
            CurrentUser.CustomerId.Value, scheduledFlightId, puestos, cancellationToken);
        return list;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<ReservationDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        // Si hay un usuario logueado en el portal de clientes, mostrar solo sus reservas
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var list = await _getByCustomer.ExecuteAsync(CurrentUser.CustomerId.Value, cancellationToken);
            return list.Select(ToDto);
        }

        var listAll = await _getAll.ExecuteAsync(cancellationToken);
        return listAll.Select(ToDto);
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

    public async Task ReprogramarReservaAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _reprogramar.ExecuteInteractiveAsync(id, cancellationToken);

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

    /// <summary>
    /// Obtiene las reservas del cliente actual logueado.
    /// </summary>
    public async Task<IEnumerable<ReservationDto>> GetMyReservationsAsync(
        CancellationToken cancellationToken = default)
    {
        if (!CurrentUser.IsAuthenticated || !CurrentUser.CustomerId.HasValue)
            throw new InvalidOperationException("Debe iniciar sesión para ver sus reservas.");

        var list = await _getByCustomer.ExecuteAsync(CurrentUser.CustomerId.Value, cancellationToken);
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
