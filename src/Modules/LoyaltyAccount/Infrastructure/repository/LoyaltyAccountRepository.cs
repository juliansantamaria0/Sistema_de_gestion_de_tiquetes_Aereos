namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class LoyaltyAccountRepository : ILoyaltyAccountRepository
{
    private readonly AppDbContext _context;

    public LoyaltyAccountRepository(AppDbContext context)
    {
        _context = context;
    }

    private static LoyaltyAccountAggregate ToDomain(LoyaltyAccountEntity entity)
        => new(
            new LoyaltyAccountId(entity.Id),
            entity.PassengerId,
            entity.LoyaltyProgramId,
            entity.LoyaltyTierId,
            entity.TotalMiles,
            entity.AvailableMiles,
            entity.JoinedAt);

    public async Task<LoyaltyAccountAggregate?> GetByIdAsync(
        LoyaltyAccountId  id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<LoyaltyAccountAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.LoyaltyAccounts
            .AsNoTracking()
            .OrderBy(e => e.PassengerId)
            .ThenBy(e => e.LoyaltyProgramId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<LoyaltyAccountAggregate?> GetByPassengerAndProgramAsync(
        int               passengerId,
        int               programId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.PassengerId == passengerId && e.LoyaltyProgramId == programId,
                cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<LoyaltyAccountAggregate>> GetByPassengerAsync(
        int               passengerId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.LoyaltyAccounts
            .AsNoTracking()
            .Where(e => e.PassengerId == passengerId)
            .OrderBy(e => e.LoyaltyProgramId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        LoyaltyAccountAggregate loyaltyAccount,
        CancellationToken       cancellationToken = default)
    {
        var entity = new LoyaltyAccountEntity
        {
            PassengerId      = loyaltyAccount.PassengerId,
            LoyaltyProgramId = loyaltyAccount.LoyaltyProgramId,
            LoyaltyTierId    = loyaltyAccount.LoyaltyTierId,
            TotalMiles       = loyaltyAccount.TotalMiles,
            AvailableMiles   = loyaltyAccount.AvailableMiles,
            JoinedAt         = loyaltyAccount.JoinedAt
        };
        await _context.LoyaltyAccounts.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        LoyaltyAccountAggregate loyaltyAccount,
        CancellationToken       cancellationToken = default)
    {
        var entity = await _context.LoyaltyAccounts
            .FirstOrDefaultAsync(e => e.Id == loyaltyAccount.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"LoyaltyAccountEntity with id {loyaltyAccount.Id.Value} not found.");

        
        entity.LoyaltyTierId  = loyaltyAccount.LoyaltyTierId;
        entity.TotalMiles     = loyaltyAccount.TotalMiles;
        entity.AvailableMiles = loyaltyAccount.AvailableMiles;

        _context.LoyaltyAccounts.Update(entity);
    }

    public async Task DeleteAsync(
        LoyaltyAccountId  id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyAccounts
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"LoyaltyAccountEntity with id {id.Value} not found.");

        _context.LoyaltyAccounts.Remove(entity);
    }
}
