namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PaymentStatusRepository : IPaymentStatusRepository
{
    private readonly AppDbContext _context;

    public PaymentStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static PaymentStatusAggregate ToDomain(PaymentStatusEntity entity)
        => new(new PaymentStatusId(entity.Id), entity.Name);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<PaymentStatusAggregate?> GetByIdAsync(
        PaymentStatusId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.PaymentStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<PaymentStatusAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.PaymentStatuses
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        PaymentStatusAggregate paymentStatus,
        CancellationToken      cancellationToken = default)
    {
        var entity = new PaymentStatusEntity { Name = paymentStatus.Name };
        await _context.PaymentStatuses.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        PaymentStatusAggregate paymentStatus,
        CancellationToken      cancellationToken = default)
    {
        var entity = await _context.PaymentStatuses
            .FirstOrDefaultAsync(e => e.Id == paymentStatus.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PaymentStatusEntity with id {paymentStatus.Id.Value} not found.");

        entity.Name = paymentStatus.Name;
        _context.PaymentStatuses.Update(entity);
    }

    public async Task DeleteAsync(
        PaymentStatusId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.PaymentStatuses
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PaymentStatusEntity with id {id.Value} not found.");

        _context.PaymentStatuses.Remove(entity);
    }
}
