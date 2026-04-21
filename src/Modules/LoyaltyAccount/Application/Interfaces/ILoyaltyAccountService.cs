namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.Interfaces;

public interface ILoyaltyAccountService
{
    Task<LoyaltyAccountDto?>             GetByIdAsync(int id,                                                                     CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyAccountDto>> GetAllAsync(                                                                             CancellationToken cancellationToken = default);
    Task<LoyaltyAccountDto?>             GetByPassengerAndProgramAsync(int passengerId, int programId,                            CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyAccountDto>> GetByPassengerAsync(int passengerId,                                                     CancellationToken cancellationToken = default);
    Task<LoyaltyAccountDto>              CreateAsync(int passengerId, int loyaltyProgramId, int loyaltyTierId,                    CancellationToken cancellationToken = default);
    Task                                 AddMilesAsync(int id, int miles,                                                         CancellationToken cancellationToken = default);
    Task                                 RedeemMilesAsync(int id, int miles,                                                      CancellationToken cancellationToken = default);
    Task                                 UpgradeTierAsync(int id, int loyaltyTierId,                                              CancellationToken cancellationToken = default);
    Task                                 DeleteAsync(int id,                                                                      CancellationToken cancellationToken = default);
}

public sealed record LoyaltyAccountDto(
    int      Id,
    int      PassengerId,
    int      LoyaltyProgramId,
    int      LoyaltyTierId,
    int      TotalMiles,
    int      AvailableMiles,
    DateTime JoinedAt);
