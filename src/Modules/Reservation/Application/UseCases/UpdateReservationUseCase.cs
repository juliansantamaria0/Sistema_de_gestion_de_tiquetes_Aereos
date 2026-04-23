namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;





public sealed class UpdateReservationUseCase
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;
    private readonly AppDbContext           _context;

    public UpdateReservationUseCase(IReservationRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task ExecuteAsync(
        int               id,
        int               reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == reservationStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {reservationStatusId}.");

        var reservation = await _repository.GetByIdAsync(new ReservationId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {id} was not found.");

        reservation.ChangeStatus(reservationStatusId);
        await _repository.UpdateAsync(reservation, cancellationToken);

        await _context.AddReservationStatusHistoryAsync(
            id,
            reservationStatusId,
            "Cambio de estado (manual)",
            DateTime.UtcNow,
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
