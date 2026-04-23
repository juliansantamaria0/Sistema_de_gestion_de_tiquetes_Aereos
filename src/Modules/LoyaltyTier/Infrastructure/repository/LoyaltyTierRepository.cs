namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class LoyaltyTierRepository : ILoyaltyTierRepository
{
    private readonly AppDbContext _context;

    public LoyaltyTierRepository(AppDbContext context)
    {
        _context = context;
    }

    private static LoyaltyTierAggregate ToDomain(LoyaltyTierEntity entity)
        => new(
            new LoyaltyTierId(entity.Id),
            entity.LoyaltyProgramId,
            entity.Name,
            entity.MinMiles,
            entity.Benefits);

    public async Task<LoyaltyTierAggregate?> GetByIdAsync(
        LoyaltyTierId     id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyTiers
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<LoyaltyTierAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.LoyaltyTiers
            .AsNoTracking()
            .OrderBy(e => e.LoyaltyProgramId)
            .ThenBy(e => e.MinMiles)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<LoyaltyTierAggregate>> GetByProgramAsync(
        int               loyaltyProgramId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.LoyaltyTiers
            .AsNoTracking()
            .Where(e => e.LoyaltyProgramId == loyaltyProgramId)
            .OrderBy(e => e.MinMiles)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        LoyaltyTierAggregate loyaltyTier,
        CancellationToken    cancellationToken = default)
    {
        var entity = new LoyaltyTierEntity
        {
            LoyaltyProgramId = loyaltyTier.LoyaltyProgramId,
            Name             = loyaltyTier.Name,
            MinMiles         = loyaltyTier.MinMiles,
            Benefits         = loyaltyTier.Benefits
        };
        await _context.LoyaltyTiers.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        LoyaltyTierAggregate loyaltyTier,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.LoyaltyTiers
            .FirstOrDefaultAsync(e => e.Id == loyaltyTier.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"LoyaltyTierEntity with id {loyaltyTier.Id.Value} not found.");

        
        entity.Name     = loyaltyTier.Name;
        entity.MinMiles = loyaltyTier.MinMiles;
        entity.Benefits = loyaltyTier.Benefits;

        _context.LoyaltyTiers.Update(entity);
    }

    public async Task DeleteAsync(
        LoyaltyTierId     id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyTiers
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"LoyaltyTierEntity with id {id.Value} not found.");

        _context.LoyaltyTiers.Remove(entity);
    }
}
