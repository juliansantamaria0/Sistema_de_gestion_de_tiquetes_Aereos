namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

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

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<ReservationAggregate?> GetByIdAsync(
        ReservationId     id,
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
        int               customerId,
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
        int               scheduledFlightId,
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
        CancellationToken    cancellationToken = default)
    {
        var entity = new ReservationEntity
        {
            ReservationCode     = reservation.ReservationCode,
            CustomerId          = reservation.CustomerId,
            ScheduledFlightId   = reservation.ScheduledFlightId,
            ReservationDate     = reservation.ReservationDate,
            ReservationStatusId = reservation.ReservationStatusId,
            ConfirmedAt         = reservation.ConfirmedAt,
            CancelledAt         = reservation.CancelledAt,
            CreatedAt           = reservation.CreatedAt,
            UpdatedAt           = reservation.UpdatedAt
        };
        await _context.Reservations.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        ReservationAggregate reservation,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.Reservations
            .FirstOrDefaultAsync(e => e.Id == reservation.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationEntity with id {reservation.Id.Value} not found.");

        // ReservationCode, CustomerId y ScheduledFlightId son inmutables.
        entity.ReservationStatusId = reservation.ReservationStatusId;
        entity.ConfirmedAt         = reservation.ConfirmedAt;
        entity.CancelledAt         = reservation.CancelledAt;
        entity.UpdatedAt           = reservation.UpdatedAt;

        _context.Reservations.Update(entity);
    }

    public async Task DeleteAsync(
        ReservationId     id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Reservations
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationEntity with id {id.Value} not found.");

        _context.Reservations.Remove(entity);
    }
}
