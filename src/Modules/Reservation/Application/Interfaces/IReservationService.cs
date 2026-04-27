namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationDto?>             GetByIdAsync(int id,                                             CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDto>> GetAllAsync(                                                     CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDto>> GetByCustomerAsync(int customerId,                               CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDto>> GetByFlightAsync(int scheduledFlightId,                          CancellationToken cancellationToken = default);
    Task<ReservationDto>              CreateAsync(int customerId, int scheduledFlightId, int reservationStatusId, bool requireAvailableSeats = true, CancellationToken cancellationToken = default);
    Task<ReservationDto>              CreateForCurrentUserAsync(int scheduledFlightId, int reservationStatusId, bool requireAvailableSeats = true, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReservationDto>> CrearSolicitudListaEsperaAsync(int scheduledFlightId, IReadOnlyList<(int PassengerId, int FareTypeId)> puestos, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDto>> GetMyReservationsAsync(                                         CancellationToken cancellationToken = default);
    Task                              ConfirmAsync(int id, int confirmedReservationStatusId,           CancellationToken cancellationToken = default);
    Task                              CancelAsync(int id, int cancelledReservationStatusId,          CancellationToken cancellationToken = default);
    Task                              ReprogramarReservaAsync(int id,                                CancellationToken cancellationToken = default);
    Task                              ChangeStatusAsync(int id, int reservationStatusId,               CancellationToken cancellationToken = default);
    Task                              DeleteAsync(int id,                                              CancellationToken cancellationToken = default);
}

public sealed record ReservationDto(
    int      Id,
    string   ReservationCode,
    int      CustomerId,
    int      ScheduledFlightId,
    DateTime ReservationDate,
    int      ReservationStatusId,
    DateTime? ConfirmedAt,
    DateTime? CancelledAt,
    DateTime  CreatedAt,
    DateTime? UpdatedAt);
