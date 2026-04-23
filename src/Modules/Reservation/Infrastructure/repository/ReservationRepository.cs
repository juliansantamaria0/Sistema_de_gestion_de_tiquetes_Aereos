namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationRepository(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    private static ReservationAggregate ToDomain(ReservationEntity entity)
        => new(
            new ReservationId(entity.Id),
            entity.ReservationCode,
            entity.CustomerId,
            entity.ScheduledFlightId,
            entity.ReservationDate,
            entity.ReservationStatusId,
            entity.ConfirmedAt,
            entity.CancelledAt,
            entity.CreatedAt,
            entity.UpdatedAt);

    public async Task<ReservationAggregate?> GetByIdAsync(
        ReservationId id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<ReservationAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Reservations
            .AsNoTracking()
            .OrderByDescending(e => e.ReservationDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<ReservationAggregate>> GetByCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Reservations
            .AsNoTracking()
            .Where(e => e.CustomerId == customerId)
            .OrderByDescending(e => e.ReservationDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<ReservationAggregate>> GetByFlightAsync(
        int scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Reservations
            .AsNoTracking()
            .Where(e => e.ScheduledFlightId == scheduledFlightId)
            .OrderByDescending(e => e.ReservationDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        ReservationAggregate reservation,
        CancellationToken cancellationToken = default)
    {
        var entity = new ReservationEntity
        {
            ReservationCode = reservation.ReservationCode,
            CustomerId = reservation.CustomerId,
            ScheduledFlightId = reservation.ScheduledFlightId,
            ReservationDate = reservation.ReservationDate,
            ReservationStatusId = reservation.ReservationStatusId,
            ConfirmedAt = reservation.ConfirmedAt,
            CancelledAt = reservation.CancelledAt,
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.UpdatedAt
        };
        await _context.Reservations.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        ReservationAggregate reservation,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Reservations
            .FirstOrDefaultAsync(e => e.Id == reservation.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationEntity with id {reservation.Id.Value} not found.");

        entity.ReservationStatusId = reservation.ReservationStatusId;
        entity.ConfirmedAt = reservation.ConfirmedAt;
        entity.CancelledAt = reservation.CancelledAt;
        entity.UpdatedAt = reservation.UpdatedAt;

        _context.Reservations.Update(entity);
    }

    public async Task DeleteAsync(
        ReservationId id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Reservations
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationEntity with id {id.Value} not found.");

        _context.Reservations.Remove(entity);
    }

    public async Task<decimal> GetQuotedFareTotalForReservationAsync(
        int reservationId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
        if (reservation is null)
            return 0m;

        var scheduledFlightId = reservation.ScheduledFlightId;

        var query =
            from d in _context.ReservationDetails.AsNoTracking()
            where d.ReservationId == reservationId
            join fs in _context.FlightSeats.AsNoTracking() on d.FlightSeatId equals fs.Id
            join sm in _context.SeatMaps.AsNoTracking() on fs.SeatMapId equals sm.Id
            join fcp in _context.FlightCabinPrices.AsNoTracking()
                on new { Sid = scheduledFlightId, sm.CabinClassId, d.FareTypeId } equals new
                {
                    Sid = fcp.ScheduledFlightId,
                    fcp.CabinClassId,
                    fcp.FareTypeId
                }
            select (decimal?)fcp.Price;

        return await query.SumAsync(cancellationToken) ?? 0m;
    }

    public async Task<ReservationAggregate> CreateReservationWithInitialHistoryAsync(
        string reservationCodeNormalized,
        int customerId,
        int scheduledFlightId,
        int reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        if (!await _context.Customers.AsNoTracking().AnyAsync(x => x.Id == customerId, cancellationToken))
            throw new InvalidOperationException($"No existe el cliente con id {customerId}.");

        if (!await _context.ScheduledFlights.AsNoTracking().AnyAsync(x => x.Id == scheduledFlightId, cancellationToken))
            throw new InvalidOperationException($"No existe el vuelo programado con id {scheduledFlightId}.");

        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == reservationStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {reservationStatusId}.");

        if (await _context.Reservations.AsNoTracking().AnyAsync(x => x.ReservationCode == reservationCodeNormalized, cancellationToken))
            throw new InvalidOperationException($"Ya existe una reserva con el código {reservationCodeNormalized}.");

        var availableSeats = await _context.FlightSeats
            .AsNoTracking()
            .Join(
                _context.SeatStatuses.AsNoTracking(),
                seat => seat.SeatStatusId,
                status => status.Id,
                (seat, status) => new { seat.ScheduledFlightId, StatusName = status.Name })
            .CountAsync(x => x.ScheduledFlightId == scheduledFlightId && x.StatusName == SeatStatusNames.Available, cancellationToken);

        if (availableSeats <= 0)
            throw new InvalidOperationException("El vuelo no tiene asientos disponibles para crear nuevas reservas.");

        var reservationEntity = new ReservationEntity
        {
            ReservationCode = reservationCodeNormalized,
            CustomerId = customerId,
            ScheduledFlightId = scheduledFlightId,
            ReservationDate = now,
            ReservationStatusId = reservationStatusId,
            ConfirmedAt = null,
            CancelledAt = null,
            CreatedAt = now,
            UpdatedAt = null
        };

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            await _context.Reservations.AddAsync(reservationEntity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _context.AddReservationStatusHistoryAsync(
                reservationEntity.Id,
                reservationStatusId,
                "Reserva creada",
                now,
                cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });

        return new ReservationAggregate(
            new ReservationId(reservationEntity.Id),
            reservationEntity.ReservationCode,
            reservationEntity.CustomerId,
            reservationEntity.ScheduledFlightId,
            reservationEntity.ReservationDate,
            reservationEntity.ReservationStatusId,
            reservationEntity.ConfirmedAt,
            reservationEntity.CancelledAt,
            reservationEntity.CreatedAt,
            reservationEntity.UpdatedAt);
    }

    public async Task PrepareConfirmReservationAsync(
        int reservationId,
        int confirmedStatusId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation with id {reservationId} was not found.");

        if (reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("Cannot confirm a reservation that has already been cancelled.");
        if (reservation.ConfirmedAt.HasValue)
            throw new InvalidOperationException("Reservation is already confirmed.");
        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == confirmedStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {confirmedStatusId}.");

        var details = await _context.ReservationDetails
            .AsNoTracking()
            .Where(x => x.ReservationId == reservationId)
            .ToListAsync(cancellationToken);

        if (details.Count == 0)
            throw new InvalidOperationException("No se puede confirmar una reserva sin detalles ni asientos asociados.");

        var detailSeatIds = details.Select(x => x.FlightSeatId).Distinct().ToList();
        var availableStatusId = await GetSeatStatusIdAsync(SeatStatusNames.Available, cancellationToken);
        var reservedStatusId = await GetSeatStatusIdAsync(SeatStatusNames.Reserved, cancellationToken);
        var occupiedStatusId = await GetSeatStatusIdAsync(SeatStatusNames.Occupied, cancellationToken);

        var seats = await _context.FlightSeats
            .Where(x => detailSeatIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (seats.Count != detailSeatIds.Count)
            throw new InvalidOperationException("La reserva contiene asientos inexistentes o eliminados.");

        if (seats.Any(x => x.ScheduledFlightId != reservation.ScheduledFlightId))
            throw new InvalidOperationException("Todos los asientos de la reserva deben pertenecer al mismo vuelo programado de la reserva.");

        var unavailableSeats = seats
            .Where(x => x.SeatStatusId != availableStatusId && x.SeatStatusId != reservedStatusId)
            .Select(x => x.Id)
            .ToList();
        if (unavailableSeats.Count > 0)
            throw new InvalidOperationException(
                $"No se puede confirmar la reserva porque estos asientos ya no están disponibles: {string.Join(", ", unavailableSeats)}.");

        var seatTakenByOtherReservation = await _context.ReservationDetails
            .AsNoTracking()
            .Join(
                _context.Reservations.AsNoTracking(),
                detail => detail.ReservationId,
                existingReservation => existingReservation.Id,
                (detail, existingReservation) => new { detail.ReservationId, detail.FlightSeatId, existingReservation.CancelledAt })
            .AnyAsync(
                x => detailSeatIds.Contains(x.FlightSeatId)
                     && x.CancelledAt == null
                     && x.ReservationId != reservationId,
                cancellationToken);

        if (seatTakenByOtherReservation)
            throw new InvalidOperationException("No se puede confirmar: hay asientos asociados a otra reserva activa.");

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
