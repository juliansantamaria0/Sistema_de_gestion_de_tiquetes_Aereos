namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class ReservationStatusHistoryRepository : IReservationStatusHistoryRepository
{
    private readonly AppDbContext _context;

    public ReservationStatusHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    private static ReservationStatusHistoryAggregate ToDomain(
        ReservationStatusHistoryEntity entity)
        => new(
            new ReservationStatusHistoryId(entity.Id),
            entity.ReservationId,
            entity.ReservationStatusId,
            entity.ChangedAt,
            entity.Notes);

    public async Task<ReservationStatusHistoryAggregate?> GetByIdAsync(
        ReservationStatusHistoryId id,
        CancellationToken          cancellationToken = default)
    {
        var entity = await _context.ReservationStatusHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<ReservationStatusHistoryAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ReservationStatusHistories
            .AsNoTracking()
            .OrderByDescending(e => e.ChangedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<ReservationStatusHistoryAggregate>> GetByReservationAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ReservationStatusHistories
            .AsNoTracking()
            .Where(e => e.ReservationId == reservationId)
            .OrderBy(e => e.ChangedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        ReservationStatusHistoryAggregate entry,
        CancellationToken                 cancellationToken = default)
    {
        var entity = new ReservationStatusHistoryEntity
        {
            ReservationId       = entry.ReservationId,
            ReservationStatusId = entry.ReservationStatusId,
            ChangedAt           = entry.ChangedAt,
            Notes               = entry.Notes
        };
        await _context.ReservationStatusHistories.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(
        ReservationStatusHistoryId id,
        CancellationToken          cancellationToken = default)
    {
        var entity = await _context.ReservationStatusHistories
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationStatusHistoryEntity with id {id.Value} not found.");

        _context.ReservationStatusHistories.Remove(entity);
    }
}
