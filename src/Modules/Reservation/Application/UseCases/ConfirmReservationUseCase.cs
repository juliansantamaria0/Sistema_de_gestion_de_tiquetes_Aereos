namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class ConfirmReservationUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmReservationUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int id,
        int confirmedStatusId,
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

        var details = await _context.ReservationDetails
            .AsNoTracking()
            .Where(x => x.ReservationId == id)
            .ToListAsync(cancellationToken);

        if (details.Count == 0)
            throw new InvalidOperationException("No se puede confirmar una reserva sin detalles ni asientos asociados.");

        var detailSeatIds = details.Select(x => x.FlightSeatId).Distinct().ToList();
        var availableStatusId = await GetSeatStatusIdAsync("AVAILABLE", cancellationToken);
        var occupiedStatusId = await GetSeatStatusIdAsync("OCCUPIED", cancellationToken);

        var seats = await _context.FlightSeats
            .Where(x => detailSeatIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (seats.Count != detailSeatIds.Count)
            throw new InvalidOperationException("La reserva contiene asientos inexistentes o eliminados.");

        if (seats.Any(x => x.ScheduledFlightId != reservation.ScheduledFlightId))
            throw new InvalidOperationException("Todos los asientos de la reserva deben pertenecer al mismo vuelo programado de la reserva.");

        var unavailableSeats = seats.Where(x => x.SeatStatusId != availableStatusId).Select(x => x.Id).ToList();
        if (unavailableSeats.Count > 0)
            throw new InvalidOperationException($"No se puede confirmar la reserva porque estos asientos ya no están disponibles: {string.Join(", ", unavailableSeats)}.");

        var now = DateTime.UtcNow;
        foreach (var seat in seats)
        {
            seat.SeatStatusId = occupiedStatusId;
            seat.UpdatedAt = now;
        }

        reservation.ReservationStatusId = confirmedStatusId;
        reservation.ConfirmedAt = now;
        reservation.UpdatedAt = now;

        await _context.AddReservationStatusHistoryAsync(
            reservation.Id,
            confirmedStatusId,
            "Reserva confirmada",
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
