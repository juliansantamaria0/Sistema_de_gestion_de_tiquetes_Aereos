namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Cancela una reserva: establece cancelled_at y actualiza el status.
/// El dominio garantiza la exclusión mutua con confirmed_at.
/// </summary>
public sealed class CancelReservationUseCase
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public CancelReservationUseCase(IReservationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               cancelledStatusId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.GetByIdAsync(new ReservationId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {id} was not found.");

        reservation.Cancel(cancelledStatusId);
        await _repository.UpdateAsync(reservation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
