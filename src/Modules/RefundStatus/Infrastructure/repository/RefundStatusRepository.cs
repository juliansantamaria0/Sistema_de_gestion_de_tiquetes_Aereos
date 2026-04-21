namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class RefundStatusRepository : IRefundStatusRepository
{
    private readonly AppDbContext _context;

    public RefundStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static RefundStatusAggregate ToDomain(RefundStatusEntity entity)
        => new(new RefundStatusId(entity.Id), entity.Name);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<RefundStatusAggregate?> GetByIdAsync(
        RefundStatusId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.RefundStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<RefundStatusAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.RefundStatuses
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        RefundStatusAggregate refundStatus,
        CancellationToken     cancellationToken = default)
    {
        var entity = new RefundStatusEntity { Name = refundStatus.Name };
        await _context.RefundStatuses.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        RefundStatusAggregate refundStatus,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.RefundStatuses
            .FirstOrDefaultAsync(e => e.Id == refundStatus.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"RefundStatusEntity with id {refundStatus.Id.Value} not found.");

        entity.Name = refundStatus.Name;
        _context.RefundStatuses.Update(entity);
    }

    public async Task DeleteAsync(
        RefundStatusId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.RefundStatuses
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"RefundStatusEntity with id {id.Value} not found.");

        _context.RefundStatuses.Remove(entity);
    }
}
