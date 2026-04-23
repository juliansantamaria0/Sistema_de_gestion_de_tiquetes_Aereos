namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class DelayReasonRepository : IDelayReasonRepository
{
    private readonly AppDbContext _context;

    public DelayReasonRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static DelayReasonAggregate ToDomain(DelayReasonEntity entity)
        => new(new DelayReasonId(entity.Id), entity.Name, entity.Category);

    

    public async Task<DelayReasonAggregate?> GetByIdAsync(
        DelayReasonId     id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.DelayReasons
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<DelayReasonAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.DelayReasons
            .AsNoTracking()
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        DelayReasonAggregate delayReason,
        CancellationToken    cancellationToken = default)
    {
        var entity = new DelayReasonEntity
        {
            Name     = delayReason.Name,
            Category = delayReason.Category
        };
        await _context.DelayReasons.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        DelayReasonAggregate delayReason,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.DelayReasons
            .FirstOrDefaultAsync(e => e.Id == delayReason.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"DelayReasonEntity with id {delayReason.Id.Value} not found.");

        entity.Name     = delayReason.Name;
        entity.Category = delayReason.Category;

        _context.DelayReasons.Update(entity);
    }

    public async Task DeleteAsync(
        DelayReasonId     id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.DelayReasons
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"DelayReasonEntity with id {id.Value} not found.");

        _context.DelayReasons.Remove(entity);
    }
}
