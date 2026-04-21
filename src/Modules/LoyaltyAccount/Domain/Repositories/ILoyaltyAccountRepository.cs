namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;

public interface ILoyaltyAccountRepository
{
    Task<LoyaltyAccountAggregate?>             GetByIdAsync(LoyaltyAccountId id,                                CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyAccountAggregate>> GetAllAsync(                                                      CancellationToken cancellationToken = default);
    Task<LoyaltyAccountAggregate?>             GetByPassengerAndProgramAsync(int passengerId, int programId,    CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyAccountAggregate>> GetByPassengerAsync(int passengerId,                             CancellationToken cancellationToken = default);
    Task                                       AddAsync(LoyaltyAccountAggregate loyaltyAccount,                 CancellationToken cancellationToken = default);
    Task                                       UpdateAsync(LoyaltyAccountAggregate loyaltyAccount,              CancellationToken cancellationToken = default);
    Task                                       DeleteAsync(LoyaltyAccountId id,                                 CancellationToken cancellationToken = default);
}
