namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly AppDbContext _context;

    public PaymentMethodRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static PaymentMethodAggregate ToDomain(PaymentMethodEntity entity)
        => new(new PaymentMethodId(entity.Id), entity.Name);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<PaymentMethodAggregate?> GetByIdAsync(
        PaymentMethodId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.PaymentMethods
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<PaymentMethodAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.PaymentMethods
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        PaymentMethodAggregate paymentMethod,
        CancellationToken      cancellationToken = default)
    {
        var entity = new PaymentMethodEntity { Name = paymentMethod.Name };
        await _context.PaymentMethods.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        PaymentMethodAggregate paymentMethod,
        CancellationToken      cancellationToken = default)
    {
        var entity = await _context.PaymentMethods
            .FirstOrDefaultAsync(e => e.Id == paymentMethod.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PaymentMethodEntity with id {paymentMethod.Id.Value} not found.");

        entity.Name = paymentMethod.Name;
        _context.PaymentMethods.Update(entity);
    }

    public async Task DeleteAsync(
        PaymentMethodId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.PaymentMethods
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PaymentMethodEntity with id {id.Value} not found.");

        _context.PaymentMethods.Remove(entity);
    }
}
