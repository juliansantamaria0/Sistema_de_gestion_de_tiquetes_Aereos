namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class CancelReservationUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int id,
        int cancelledStatusId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {id} was not found.");

        if (reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("Reservation is already cancelled.");
        if (reservation.ConfirmedAt.HasValue)
            throw new InvalidOperationException("Cannot cancel a reservation that has already been confirmed.");
        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == cancelledStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {cancelledStatusId}.");

        var detailSeatIds = await _context.ReservationDetails
            .AsNoTracking()
            .Where(x => x.ReservationId == id)
            .Select(x => x.FlightSeatId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var reservationDetailIds = await _context.ReservationDetails
            .AsNoTracking()
            .Where(x => x.ReservationId == id)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (reservationDetailIds.Count > 0)
        {
            var hasIssuedTickets = await _context.Tickets
                .AsNoTracking()
                .AnyAsync(x => reservationDetailIds.Contains(x.ReservationDetailId), cancellationToken);

            if (hasIssuedTickets)
                throw new InvalidOperationException("No se puede cancelar una reserva que ya tiene tiquetes emitidos.");
        }

        if (detailSeatIds.Count > 0)
        {
            var availableStatusId = await GetSeatStatusIdAsync(SeatStatusNames.Available, cancellationToken);
            var seats = await _context.FlightSeats
                .Where(x => detailSeatIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var nowForSeats = DateTime.UtcNow;
            foreach (var seat in seats)
            {
                seat.SeatStatusId = availableStatusId;
                seat.UpdatedAt = nowForSeats;
            }
        }

        var now = DateTime.UtcNow;
        reservation.ReservationStatusId = cancelledStatusId;
        reservation.CancelledAt = now;
        reservation.UpdatedAt = now;

        await _context.AddReservationStatusHistoryAsync(
            reservation.Id,
            cancelledStatusId,
            reservation.ConfirmedAt.HasValue
                ? "Reserva cancelada y asientos liberados"
                : "Reserva cancelada",
            now,
            cancellationToken);

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
