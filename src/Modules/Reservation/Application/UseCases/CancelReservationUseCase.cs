namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Cancela una reserva: establece cancelled_at y actualiza el status.
/// Además registra trazabilidad y libera los asientos vinculados si existían.
/// </summary>
public sealed class CancelReservationUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork  _unitOfWork;

    public CancelReservationUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               cancelledStatusId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {id} was not found.");

        if (reservation.ConfirmedAt.HasValue)
            throw new InvalidOperationException("Cannot cancel a reservation that has already been confirmed.");
        if (reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("Reservation is already cancelled.");
        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == cancelledStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {cancelledStatusId}.");

        var detailSeatIds = await _context.ReservationDetails
            .AsNoTracking()
            .Where(x => x.ReservationId == id)
            .Select(x => x.FlightSeatId)
            .ToListAsync(cancellationToken);

        if (detailSeatIds.Count > 0)
        {
            var availableStatusId = await GetSeatStatusIdAsync("AVAILABLE", cancellationToken);
            var seats = await _context.FlightSeats
                .Where(x => detailSeatIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            foreach (var seat in seats)
            {
                seat.SeatStatusId = availableStatusId;
                seat.UpdatedAt = DateTime.UtcNow;
            }
        }

        reservation.ReservationStatusId = cancelledStatusId;
        reservation.CancelledAt = DateTime.UtcNow;
        reservation.UpdatedAt = reservation.CancelledAt;

        await _context.ReservationStatusHistories.AddAsync(new ReservationStatusHistoryEntity
        {
            ReservationId = reservation.Id,
            ReservationStatusId = cancelledStatusId,
            ChangedAt = DateTime.UtcNow,
            Notes = "Reserva cancelada"
        }, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task<int> GetSeatStatusIdAsync(string name, CancellationToken cancellationToken)
    {
        var id = await _context.SeatStatuses
            .AsNoTracking()
            .Where(x => x.Name == name)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (id <= 0)
            throw new InvalidOperationException($"No existe el estado de asiento '{name}'.");

        return id;
    }
}
