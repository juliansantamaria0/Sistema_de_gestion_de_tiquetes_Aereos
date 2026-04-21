namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Confirma una reserva: establece confirmed_at y actualiza el status.
/// El dominio garantiza la exclusión mutua con cancelled_at.
/// </summary>
public sealed class ConfirmReservationUseCase
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public ConfirmReservationUseCase(IReservationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               confirmedStatusId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.GetByIdAsync(new ReservationId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {id} was not found.");

        reservation.Confirm(confirmedStatusId);
        await _repository.UpdateAsync(reservation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
