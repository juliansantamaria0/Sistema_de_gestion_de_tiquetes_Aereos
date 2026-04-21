namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;

    public PromotionRepository(AppDbContext context) => _context = context;

    private static PromotionAggregate ToDomain(PromotionEntity e)
        => new(new PromotionId(e.Id), e.AirlineId, e.Name,
               e.DiscountPct, e.ValidFrom, e.ValidUntil);

    public async Task<PromotionAggregate?> GetByIdAsync(
        PromotionId id, CancellationToken ct = default)
    {
        var e = await _context.Promotions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<PromotionAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.Promotions.AsNoTracking()
            .OrderBy(x => x.ValidFrom).ThenBy(x => x.AirlineId)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<PromotionAggregate>> GetByAirlineAsync(
        int airlineId, CancellationToken ct = default)
    {
        var entities = await _context.Promotions.AsNoTracking()
            .Where(x => x.AirlineId == airlineId)
            .OrderBy(x => x.ValidFrom)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<PromotionAggregate>> GetActiveAsync(
        DateOnly referenceDate, CancellationToken ct = default)
    {
        // Espejo de: valid_from <= referenceDate <= valid_until
        var entities = await _context.Promotions.AsNoTracking()
            .Where(x => x.ValidFrom <= referenceDate && x.ValidUntil >= referenceDate)
            .OrderBy(x => x.AirlineId).ThenBy(x => x.Name)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(PromotionAggregate promotion, CancellationToken ct = default)
    {
        await _context.Promotions.AddAsync(new PromotionEntity
        {
            AirlineId   = promotion.AirlineId,
            Name        = promotion.Name,
            DiscountPct = promotion.DiscountPct,
            ValidFrom   = promotion.ValidFrom,
            ValidUntil  = promotion.ValidUntil
        }, ct);
    }

    public async Task UpdateAsync(PromotionAggregate promotion, CancellationToken ct = default)
    {
        var e = await _context.Promotions
            .FirstOrDefaultAsync(x => x.Id == promotion.Id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"PromotionEntity with id {promotion.Id.Value} not found.");

        // AirlineId es la clave de contexto — inmutable.
        e.Name        = promotion.Name;
        e.DiscountPct = promotion.DiscountPct;
        e.ValidFrom   = promotion.ValidFrom;
        e.ValidUntil  = promotion.ValidUntil;
        _context.Promotions.Update(e);
    }

    public async Task DeleteAsync(PromotionId id, CancellationToken ct = default)
    {
        var e = await _context.Promotions
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"PromotionEntity with id {id.Value} not found.");
        _context.Promotions.Remove(e);
    }
}
