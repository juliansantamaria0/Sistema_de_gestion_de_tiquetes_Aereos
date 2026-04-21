namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class RefundRepository : IRefundRepository
{
    private readonly AppDbContext _context;

    public RefundRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static RefundAggregate ToDomain(RefundEntity entity)
        => new(
            new RefundId(entity.Id),
            entity.PaymentId,
            entity.RefundStatusId,
            entity.Amount,
            entity.RequestedAt,
            entity.ProcessedAt,
            entity.Reason);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<RefundAggregate?> GetByIdAsync(
        RefundId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Refunds
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<RefundAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Refunds
            .AsNoTracking()
            .OrderByDescending(e => e.RequestedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<RefundAggregate>> GetByPaymentAsync(
        int               paymentId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Refunds
            .AsNoTracking()
            .Where(e => e.PaymentId == paymentId)
            .OrderByDescending(e => e.RequestedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        RefundAggregate   refund,
        CancellationToken cancellationToken = default)
    {
        var entity = new RefundEntity
        {
            PaymentId      = refund.PaymentId,
            RefundStatusId = refund.RefundStatusId,
            Amount         = refund.Amount,
            RequestedAt    = refund.RequestedAt,
            ProcessedAt    = refund.ProcessedAt,
            Reason         = refund.Reason
        };
        await _context.Refunds.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        RefundAggregate   refund,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Refunds
            .FirstOrDefaultAsync(e => e.Id == refund.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"RefundEntity with id {refund.Id.Value} not found.");

        // Solo RefundStatusId, ProcessedAt y Reason son mutables.
        // PaymentId, Amount y RequestedAt son inmutables.
        entity.RefundStatusId = refund.RefundStatusId;
        entity.ProcessedAt    = refund.ProcessedAt;
        entity.Reason         = refund.Reason;

        _context.Refunds.Update(entity);
    }

    public async Task DeleteAsync(
        RefundId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Refunds
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"RefundEntity with id {id.Value} not found.");

        _context.Refunds.Remove(entity);
    }
}
