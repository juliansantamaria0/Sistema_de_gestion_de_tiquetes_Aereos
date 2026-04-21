namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateReservationUseCase
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public CreateReservationUseCase(IReservationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationAggregate> ExecuteAsync(
        string            reservationCode,
        int               customerId,
        int               scheduledFlightId,
        int               reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // ReservationId(1) es placeholder; EF Core asigna el Id real al insertar.
        var reservation = new ReservationAggregate(
            new ReservationId(1),
            reservationCode,
            customerId,
            scheduledFlightId,
            now,
            reservationStatusId,
            confirmedAt: null,
            cancelledAt: null,
            createdAt:   now);

        await _repository.AddAsync(reservation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return reservation;
    }
}
