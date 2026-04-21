namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Cambia el estado de la reserva sin modificar confirmed_at ni cancelled_at.
/// Para confirmar o cancelar usar ConfirmReservationUseCase / CancelReservationUseCase.
/// </summary>
public sealed class UpdateReservationUseCase
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public UpdateReservationUseCase(IReservationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.GetByIdAsync(new ReservationId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {id} was not found.");

        reservation.ChangeStatus(reservationStatusId);
        await _repository.UpdateAsync(reservation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
