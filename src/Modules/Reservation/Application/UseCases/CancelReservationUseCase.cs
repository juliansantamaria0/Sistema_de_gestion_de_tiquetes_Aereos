namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Services;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Spectre.Console;

public sealed class CancelReservationUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWaitlistPromotionService _waitlistPromotion;

    public CancelReservationUseCase(
        AppDbContext context,
        IUnitOfWork unitOfWork,
        IWaitlistPromotionService waitlistPromotion)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _waitlistPromotion = waitlistPromotion;
    }

    public async Task ExecuteAsync(
        int id,
        int cancelledStatusId,
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var efTx = await _context.Database.BeginTransactionAsync(cancellationToken);

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw new KeyNotFoundException($"No se encontró la reserva con id {id}.");

            if (reservation.CancelledAt.HasValue)
                throw new InvalidOperationException("La reserva ya está cancelada.");
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

            // Importante: si liberamos asientos, también eliminamos los detalles (si no, el asiento queda "disponible"
            // pero sigue asociado a un reservation_detail, lo que puede impedir la promoción desde lista de espera).
            if (reservationDetailIds.Count > 0)
            {
                await _context.ReservationDetails
                    .Where(x => x.ReservationId == id)
                    .ExecuteDeleteAsync(cancellationToken);
            }

            var now = DateTime.UtcNow;
            reservation.ReservationStatusId = cancelledStatusId;
            reservation.CancelledAt = now;
            // Si una reserva estaba confirmada y luego se cancela, no puede conservar confirmed_at.
            reservation.ConfirmedAt = null;
            reservation.UpdatedAt = now;

            await _context.AddReservationStatusHistoryAsync(
                reservation.Id,
                cancelledStatusId,
                "Reserva cancelada y asientos liberados",
                now,
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            if (detailSeatIds.Count > 0)
            {
                try
                {
                    var dbConn = _context.Database.GetDbConnection();
                    if (dbConn is not MySqlConnection connection)
                        throw new InvalidOperationException("La conexión actual no es MySQL.");
                    if (connection.State != System.Data.ConnectionState.Open)
                        await connection.OpenAsync(cancellationToken);
                    var dbTx = _context.Database.CurrentTransaction?.GetDbTransaction();
                    if (dbTx is not MySqlTransaction tx)
                        throw new InvalidOperationException("Se requiere transacción MySQL activa para la promoción desde lista de espera.");

                    await _waitlistPromotion.PromotePendingReservationsForFlightAsync(
                        connection,
                        tx,
                        reservation.ScheduledFlightId,
                        code => AnsiConsole.MarkupLine(
                            $"[green]PROMOCIÓN[/] Reserva [bold]{Markup.Escape(code)}[/] " +
                            $"promovida desde lista de espera al vuelo #{reservation.ScheduledFlightId}."),
                        cancellationToken);
                }
                catch (MySqlException ex)
                {
                    throw new InvalidOperationException(MySqlErrorFormatter.ToUserMessage(ex), ex);
                }
            }

            await efTx.CommitAsync(cancellationToken);
        });
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
