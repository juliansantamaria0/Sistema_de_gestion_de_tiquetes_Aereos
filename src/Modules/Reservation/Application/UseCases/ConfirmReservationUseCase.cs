namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class ConfirmReservationUseCase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmReservationUseCase(IReservationRepository reservationRepository, IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int id,
        int confirmedStatusId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _reservationRepository.PrepareConfirmReservationAsync(id, confirmedStatusId, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("El asiento ya fue confirmado por alguien más.");
        }
    }
}
