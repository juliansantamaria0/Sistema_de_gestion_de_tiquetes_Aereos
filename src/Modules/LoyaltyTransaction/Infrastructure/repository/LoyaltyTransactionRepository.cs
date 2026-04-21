namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class LoyaltyTransactionRepository : ILoyaltyTransactionRepository
{
    private readonly AppDbContext _context;

    public LoyaltyTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    private static LoyaltyTransactionAggregate ToDomain(LoyaltyTransactionEntity entity)
        => new(
            new LoyaltyTransactionId(entity.Id),
            entity.LoyaltyAccountId,
            entity.TicketId,
            entity.TransactionType,
            entity.Miles,
            entity.TransactionDate);

    public async Task<LoyaltyTransactionAggregate?> GetByIdAsync(
        LoyaltyTransactionId id,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.LoyaltyTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<LoyaltyTransactionAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.LoyaltyTransactions
            .AsNoTracking()
            .OrderByDescending(e => e.TransactionDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<LoyaltyTransactionAggregate>> GetByAccountAsync(
        int               loyaltyAccountId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.LoyaltyTransactions
            .AsNoTracking()
            .Where(e => e.LoyaltyAccountId == loyaltyAccountId)
            .OrderByDescending(e => e.TransactionDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        LoyaltyTransactionAggregate loyaltyTransaction,
        CancellationToken           cancellationToken = default)
    {
        var entity = new LoyaltyTransactionEntity
        {
            LoyaltyAccountId = loyaltyTransaction.LoyaltyAccountId,
            TicketId         = loyaltyTransaction.TicketId,
            TransactionType  = loyaltyTransaction.TransactionType,
            Miles            = loyaltyTransaction.Miles,
            TransactionDate  = loyaltyTransaction.TransactionDate
        };
        await _context.LoyaltyTransactions.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(
        LoyaltyTransactionId id,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.LoyaltyTransactions
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"LoyaltyTransactionEntity with id {id.Value} not found.");

        _context.LoyaltyTransactions.Remove(entity);
    }
}
