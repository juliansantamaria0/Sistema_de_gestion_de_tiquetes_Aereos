namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.Interfaces;

public interface ILoyaltyTierService
{
    Task<LoyaltyTierDto?>             GetByIdAsync(int id,                                                                    CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTierDto>> GetAllAsync(                                                                            CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTierDto>> GetByProgramAsync(int loyaltyProgramId,                                                 CancellationToken cancellationToken = default);
    Task<LoyaltyTierDto>              CreateAsync(int loyaltyProgramId, string name, int minMiles, string? benefits,          CancellationToken cancellationToken = default);
    Task                              UpdateAsync(int id, string name, int minMiles, string? benefits,                        CancellationToken cancellationToken = default);
    Task                              DeleteAsync(int id,                                                                     CancellationToken cancellationToken = default);
}

public sealed record LoyaltyTierDto(
    int     Id,
    int     LoyaltyProgramId,
    string  Name,
    int     MinMiles,
    string? Benefits);
