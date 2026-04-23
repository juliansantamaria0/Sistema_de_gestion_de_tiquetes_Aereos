namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;

public sealed class CreateReservationUseCase
{
    private readonly IReservationRepository _reservationRepository;

    public CreateReservationUseCase(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public Task<ReservationAggregate> ExecuteAsync(
        string reservationCode,
        int customerId,
        int scheduledFlightId,
        int reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = reservationCode.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new InvalidOperationException("El código de reserva es obligatorio.");

        return _reservationRepository.CreateReservationWithInitialHistoryAsync(
            normalizedCode,
            customerId,
            scheduledFlightId,
            reservationStatusId,
            cancellationToken);
    }
}
