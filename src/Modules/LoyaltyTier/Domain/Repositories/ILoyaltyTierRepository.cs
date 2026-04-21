namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;

public interface ILoyaltyTierRepository
{
    Task<LoyaltyTierAggregate?>             GetByIdAsync(LoyaltyTierId id,                              CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTierAggregate>> GetAllAsync(                                                 CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTierAggregate>> GetByProgramAsync(int loyaltyProgramId,                      CancellationToken cancellationToken = default);
    Task                                    AddAsync(LoyaltyTierAggregate loyaltyTier,                   CancellationToken cancellationToken = default);
    Task                                    UpdateAsync(LoyaltyTierAggregate loyaltyTier,                CancellationToken cancellationToken = default);
    Task                                    DeleteAsync(LoyaltyTierId id,                                CancellationToken cancellationToken = default);
}
