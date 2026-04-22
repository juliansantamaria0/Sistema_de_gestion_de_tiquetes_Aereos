namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Confirma una reserva: establece confirmed_at y actualiza el status.
/// Además registra trazabilidad y sincroniza disponibilidad de asientos
/// vinculados a la reserva a OCCUPIED.
/// </summary>
public sealed class ConfirmReservationUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork  _unitOfWork;

    public ConfirmReservationUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               confirmedStatusId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {id} was not found.");

        if (reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("Cannot confirm a reservation that has already been cancelled.");
        if (reservation.ConfirmedAt.HasValue)
            throw new InvalidOperationException("Reservation is already confirmed.");
        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == confirmedStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {confirmedStatusId}.");

        var detailSeatIds = await _context.ReservationDetails
            .AsNoTracking()
            .Where(x => x.ReservationId == id)
            .Select(x => x.FlightSeatId)
            .ToListAsync(cancellationToken);

        if (detailSeatIds.Count > 0)
        {
            var occupiedStatusId = await GetSeatStatusIdAsync("OCCUPIED", cancellationToken);
            var seats = await _context.FlightSeats
                .Where(x => detailSeatIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            foreach (var seat in seats)
            {
                seat.SeatStatusId = occupiedStatusId;
                seat.UpdatedAt = DateTime.UtcNow;
            }
        }

        reservation.ReservationStatusId = confirmedStatusId;
        reservation.ConfirmedAt = DateTime.UtcNow;
        reservation.UpdatedAt = reservation.ConfirmedAt;

        await _context.ReservationStatusHistories.AddAsync(new ReservationStatusHistoryEntity
        {
            ReservationId = reservation.Id,
            ReservationStatusId = confirmedStatusId,
            ChangedAt = DateTime.UtcNow,
            Notes = "Reserva confirmada"
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
