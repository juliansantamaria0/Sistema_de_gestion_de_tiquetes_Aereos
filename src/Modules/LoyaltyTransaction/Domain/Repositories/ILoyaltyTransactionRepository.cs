namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.ValueObject;

public interface ILoyaltyTransactionRepository
{
    Task<LoyaltyTransactionAggregate?>             GetByIdAsync(LoyaltyTransactionId id,                     CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTransactionAggregate>> GetAllAsync(                                               CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTransactionAggregate>> GetByAccountAsync(int loyaltyAccountId,                   CancellationToken cancellationToken = default);
    Task                                           AddAsync(LoyaltyTransactionAggregate loyaltyTransaction,  CancellationToken cancellationToken = default);
    Task                                           DeleteAsync(LoyaltyTransactionId id,                      CancellationToken cancellationToken = default);
    
}
