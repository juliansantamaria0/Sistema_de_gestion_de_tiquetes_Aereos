namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

/// <summary>
/// Vuelo sin cupo: crea N reservas en WAITLIST (una por plaza) e inserta N filas en <c>lista_espera</c>
/// ordenadas por <c>fecha_solicitud</c> (prioridad de llegada).
/// </summary>
public sealed class CrearSolicitudListaEsperaUseCase
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private readonly AppDbContext            _context;
    private readonly IReservationRepository  _reservationRepository;
    private readonly IWaitlistRepository     _waitlist;

    public CrearSolicitudListaEsperaUseCase(
        AppDbContext context,
        IReservationRepository reservationRepository,
        IWaitlistRepository waitlist)
    {
        _context = context;
        _reservationRepository = reservationRepository;
        _waitlist = waitlist;
    }

    public async Task<IReadOnlyList<ReservationDto>> ExecuteAsync(
        int customerId,
        int scheduledFlightId,
        IReadOnlyList<(int PassengerId, int FareTypeId)> puestos,
        CancellationToken cancellationToken = default)
    {
        if (puestos is null || puestos.Count == 0)
            throw new InvalidOperationException("Debe indicar al menos un pasajero y tipo de tarifa.");

        if (puestos.Count != puestos.Select(p => p.PassengerId).Distinct().Count())
            throw new InvalidOperationException("Cada plaza debe corresponder a un pasajero distinto (sin duplicados).");

        if (!CurrentUser.IsAuthenticated || CurrentUser.CustomerId != customerId)
            throw new InvalidOperationException("Debe iniciar sesión con la misma cuenta de cliente que realiza la solicitud.");

        var availableCount = await _context.FlightSeats.AsNoTracking()
            .Join(
                _context.SeatStatuses.AsNoTracking(),
                s => s.SeatStatusId,
                st => st.Id,
                (s, st) => new { s.ScheduledFlightId, st.Name })
            .CountAsync(x => x.ScheduledFlightId == scheduledFlightId && x.Name == SeatStatusNames.Available, cancellationToken);

        if (availableCount > 0)
            throw new InvalidOperationException("Aún hay asientos libres: use el flujo de reserva con cupo available.");

        var waitlistStatusId = await _context.ReservationStatuses.AsNoTracking()
            .Where(s => s.Name == "WAITLIST")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (waitlistStatusId == 0)
            throw new InvalidOperationException("Falta el estado de reserva 'WAITLIST' en el catálogo.");

        var customerPersonId = await _context.Customers.AsNoTracking()
            .Where(c => c.Id == customerId)
            .Select(c => c.PersonId)
            .FirstOrDefaultAsync(cancellationToken);
        if (customerPersonId == 0)
            throw new InvalidOperationException("Cliente no encontrado.");

        var paxIds = puestos.Select(p => p.PassengerId).Distinct().ToList();
        var allowedPax = await _context.Passengers.AsNoTracking()
            .Where(p => paxIds.Contains(p.Id) && p.PersonId == customerPersonId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
        if (allowedPax.Count != paxIds.Count)
            throw new InvalidOperationException("Solo puede incluir pasajeros vinculados a su persona de cliente.");

        foreach (var p in puestos)
        {
            var okFare = await _context.FlightCabinPrices.AsNoTracking()
                .AnyAsync(f => f.ScheduledFlightId == scheduledFlightId && f.FareTypeId == p.FareTypeId, cancellationToken);
            if (!okFare)
                throw new InvalidOperationException(
                    $"El tipo de tarifa (id {p.FareTypeId}) no aplica a este vuelo.");
        }

        var created = new List<ReservationDto>();
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
            var conn = (MySqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);
            var dbTr = _context.Database.CurrentTransaction?.GetDbTransaction();
            if (dbTr is not MySqlTransaction mTx)
                throw new InvalidOperationException("Se requiere transacción MySQL (misma conexión del DbContext).");

            try
            {
                foreach (var (passengerId, fareTypeId) in puestos)
                {
                    if (await _waitlist.HasPassengerPendingOnFlightAsync(
                            conn, mTx, scheduledFlightId, passengerId, cancellationToken))
                        throw new InvalidOperationException(
                            "Ese pasajero ya está en lista de espera para este vuelo (solicitud pendiente).");

                    var code = await GenerateUniqueCodeAsync(cancellationToken);
                    var agg = await _reservationRepository.CreateReservationWithInitialHistoryAsync(
                        code, customerId, scheduledFlightId, waitlistStatusId, requireAvailableSeats: false, cancellationToken);

                    var prioridad = await _waitlist.GetNextPriorityAsync(conn, mTx, scheduledFlightId, cancellationToken);
                    await _waitlist.InsertPendingAsync(
                        conn, mTx,
                        reservationId: agg.Id.Value,
                        scheduledFlightId,
                        passengerId,
                        fareTypeId,
                        prioridad,
                        fechaSolicitudUtc: DateTime.UtcNow,
                        ct: cancellationToken);

                    created.Add(
                        new ReservationDto(
                            agg.Id.Value, agg.ReservationCode, agg.CustomerId, agg.ScheduledFlightId, agg.ReservationDate,
                            agg.ReservationStatusId, agg.ConfirmedAt, agg.CancelledAt, agg.CreatedAt, agg.UpdatedAt));
                }

                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return created;
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct)
    {
        while (true)
        {
            var code = new string(Enumerable.Range(0, 6)
                .Select(_ => Chars[Random.Shared.Next(Chars.Length)])
                .ToArray());
            if (!await _context.Reservations.AsNoTracking().AnyAsync(r => r.ReservationCode == code, ct))
                return code;
        }
    }
}
