namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FareTypeRepository : IFareTypeRepository
{
    private readonly AppDbContext _context;

    public FareTypeRepository(AppDbContext context) => _context = context;

    private static FareTypeAggregate ToDomain(FareTypeEntity e)
        => new(new FareTypeId(e.Id), e.Name, e.IsRefundable, e.IsChangeable,
               e.AdvancePurchaseDays, e.BaggageIncluded);

    public async Task<FareTypeAggregate?> GetByIdAsync(
        FareTypeId id, CancellationToken ct = default)
    {
        var e = await _context.FareTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<FareTypeAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.FareTypes.AsNoTracking()
            .OrderBy(x => x.Name).ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(FareTypeAggregate fareType, CancellationToken ct = default)
    {
        await _context.FareTypes.AddAsync(new FareTypeEntity
        {
            Name                = fareType.Name,
            IsRefundable        = fareType.IsRefundable,
            IsChangeable        = fareType.IsChangeable,
            AdvancePurchaseDays = fareType.AdvancePurchaseDays,
            BaggageIncluded     = fareType.BaggageIncluded
        }, ct);
    }

    public async Task UpdateAsync(FareTypeAggregate fareType, CancellationToken ct = default)
    {
        var e = await _context.FareTypes
            .FirstOrDefaultAsync(x => x.Id == fareType.Id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"FareTypeEntity with id {fareType.Id.Value} not found.");

        e.Name                = fareType.Name;
        e.IsRefundable        = fareType.IsRefundable;
        e.IsChangeable        = fareType.IsChangeable;
        e.AdvancePurchaseDays = fareType.AdvancePurchaseDays;
        e.BaggageIncluded     = fareType.BaggageIncluded;
        _context.FareTypes.Update(e);
    }

    public async Task DeleteAsync(FareTypeId id, CancellationToken ct = default)
    {
        var e = await _context.FareTypes
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"FareTypeEntity with id {id.Value} not found.");
        _context.FareTypes.Remove(e);
    }
}
