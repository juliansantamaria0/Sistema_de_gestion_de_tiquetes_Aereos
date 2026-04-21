namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class TicketStatusHistoryRepository : ITicketStatusHistoryRepository
{
    private readonly AppDbContext _context;

    public TicketStatusHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    private static TicketStatusHistoryAggregate ToDomain(TicketStatusHistoryEntity entity)
        => new(
            new TicketStatusHistoryId(entity.Id),
            entity.TicketId,
            entity.TicketStatusId,
            entity.ChangedAt,
            entity.Notes);

    public async Task<TicketStatusHistoryAggregate?> GetByIdAsync(
        TicketStatusHistoryId id,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.TicketStatusHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<TicketStatusHistoryAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.TicketStatusHistories
            .AsNoTracking()
            .OrderByDescending(e => e.ChangedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<TicketStatusHistoryAggregate>> GetByTicketAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.TicketStatusHistories
            .AsNoTracking()
            .Where(e => e.TicketId == ticketId)
            .OrderBy(e => e.ChangedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        TicketStatusHistoryAggregate entry,
        CancellationToken            cancellationToken = default)
    {
        var entity = new TicketStatusHistoryEntity
        {
            TicketId       = entry.TicketId,
            TicketStatusId = entry.TicketStatusId,
            ChangedAt      = entry.ChangedAt,
            Notes          = entry.Notes
        };
        await _context.TicketStatusHistories.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(
        TicketStatusHistoryId id,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.TicketStatusHistories
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketStatusHistoryEntity with id {id.Value} not found.");

        _context.TicketStatusHistories.Remove(entity);
    }
}
