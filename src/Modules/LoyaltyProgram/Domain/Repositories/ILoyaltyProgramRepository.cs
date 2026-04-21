namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;

public interface ILoyaltyProgramRepository
{
    Task<LoyaltyProgramAggregate?>             GetByIdAsync(LoyaltyProgramId id,                     CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyProgramAggregate>> GetAllAsync(                                           CancellationToken cancellationToken = default);
    Task<LoyaltyProgramAggregate?>             GetByAirlineAsync(int airlineId,                       CancellationToken cancellationToken = default);
    Task                                       AddAsync(LoyaltyProgramAggregate loyaltyProgram,       CancellationToken cancellationToken = default);
    Task                                       UpdateAsync(LoyaltyProgramAggregate loyaltyProgram,    CancellationToken cancellationToken = default);
    Task                                       DeleteAsync(LoyaltyProgramId id,                       CancellationToken cancellationToken = default);
}
