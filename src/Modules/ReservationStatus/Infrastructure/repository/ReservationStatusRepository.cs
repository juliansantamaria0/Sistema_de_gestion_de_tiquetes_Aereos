namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class ReservationStatusRepository : IReservationStatusRepository
{
    private readonly AppDbContext _context;

    public ReservationStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static ReservationStatusAggregate ToDomain(ReservationStatusEntity entity)
        => new(new ReservationStatusId(entity.Id), entity.Name);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<ReservationStatusAggregate?> GetByIdAsync(
        ReservationStatusId id,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.ReservationStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<ReservationStatusAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ReservationStatuses
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        ReservationStatusAggregate reservationStatus,
        CancellationToken          cancellationToken = default)
    {
        var entity = new ReservationStatusEntity { Name = reservationStatus.Name };
        await _context.ReservationStatuses.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        ReservationStatusAggregate reservationStatus,
        CancellationToken          cancellationToken = default)
    {
        var entity = await _context.ReservationStatuses
            .FirstOrDefaultAsync(e => e.Id == reservationStatus.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationStatusEntity with id {reservationStatus.Id.Value} not found.");

        entity.Name = reservationStatus.Name;
        _context.ReservationStatuses.Update(entity);
    }

    public async Task DeleteAsync(
        ReservationStatusId id,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.ReservationStatuses
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationStatusEntity with id {id.Value} not found.");

        _context.ReservationStatuses.Remove(entity);
    }
}
