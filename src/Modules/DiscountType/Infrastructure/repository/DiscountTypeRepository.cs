namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class DiscountTypeRepository : IDiscountTypeRepository
{
    private readonly AppDbContext _context;

    public DiscountTypeRepository(AppDbContext context) => _context = context;

    private static DiscountTypeAggregate ToDomain(DiscountTypeEntity e)
        => new(new DiscountTypeId(e.Id), e.Name, e.Percentage, e.AgeMin, e.AgeMax);

    public async Task<DiscountTypeAggregate?> GetByIdAsync(
        DiscountTypeId id, CancellationToken ct = default)
    {
        var e = await _context.DiscountTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<DiscountTypeAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.DiscountTypes.AsNoTracking()
            .OrderBy(x => x.Name).ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(DiscountTypeAggregate dt, CancellationToken ct = default)
    {
        await _context.DiscountTypes.AddAsync(new DiscountTypeEntity
        {
            Name       = dt.Name,
            Percentage = dt.Percentage,
            AgeMin     = dt.AgeMin,
            AgeMax     = dt.AgeMax
        }, ct);
    }

    public async Task UpdateAsync(DiscountTypeAggregate dt, CancellationToken ct = default)
    {
        var e = await _context.DiscountTypes
            .FirstOrDefaultAsync(x => x.Id == dt.Id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"DiscountTypeEntity with id {dt.Id.Value} not found.");

        e.Name       = dt.Name;
        e.Percentage = dt.Percentage;
        e.AgeMin     = dt.AgeMin;
        e.AgeMax     = dt.AgeMax;
        _context.DiscountTypes.Update(e);
    }

    public async Task DeleteAsync(DiscountTypeId id, CancellationToken ct = default)
    {
        var e = await _context.DiscountTypes
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"DiscountTypeEntity with id {id.Value} not found.");
        _context.DiscountTypes.Remove(e);
    }
}
