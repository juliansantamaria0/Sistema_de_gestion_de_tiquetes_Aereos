namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;

public sealed class LoyaltyAccountService : ILoyaltyAccountService
{
    private readonly CreateLoyaltyAccountUseCase            _create;
    private readonly DeleteLoyaltyAccountUseCase            _delete;
    private readonly GetAllLoyaltyAccountsUseCase           _getAll;
    private readonly GetLoyaltyAccountByIdUseCase           _getById;
    private readonly AddMilesUseCase                        _addMiles;
    private readonly RedeemMilesUseCase                     _redeemMiles;
    private readonly UpgradeTierUseCase                     _upgradeTier;
    private readonly GetLoyaltyAccountsByPassengerUseCase   _getByPassenger;

    public LoyaltyAccountService(
        CreateLoyaltyAccountUseCase          create,
        DeleteLoyaltyAccountUseCase          delete,
        GetAllLoyaltyAccountsUseCase         getAll,
        GetLoyaltyAccountByIdUseCase         getById,
        AddMilesUseCase                      addMiles,
        RedeemMilesUseCase                   redeemMiles,
        UpgradeTierUseCase                   upgradeTier,
        GetLoyaltyAccountsByPassengerUseCase getByPassenger)
    {
        _create         = create;
        _delete         = delete;
        _getAll         = getAll;
        _getById        = getById;
        _addMiles       = addMiles;
        _redeemMiles    = redeemMiles;
        _upgradeTier    = upgradeTier;
        _getByPassenger = getByPassenger;
    }

    public async Task<LoyaltyAccountDto> CreateAsync(
        int               passengerId,
        int               loyaltyProgramId,
        int               loyaltyTierId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            passengerId, loyaltyProgramId, loyaltyTierId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<LoyaltyAccountDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<LoyaltyAccountDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task AddMilesAsync(int id, int miles, CancellationToken cancellationToken = default)
        => await _addMiles.ExecuteAsync(id, miles, cancellationToken);

    public async Task RedeemMilesAsync(int id, int miles, CancellationToken cancellationToken = default)
        => await _redeemMiles.ExecuteAsync(id, miles, cancellationToken);

    public async Task UpgradeTierAsync(int id, int loyaltyTierId, CancellationToken cancellationToken = default)
        => await _upgradeTier.ExecuteAsync(id, loyaltyTierId, cancellationToken);

    public async Task<LoyaltyAccountDto?> GetByPassengerAndProgramAsync(
        int               passengerId,
        int               programId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByPassenger.ExecuteAsync(passengerId, cancellationToken);
        var found = agg.FirstOrDefault(a => a.LoyaltyProgramId == programId);
        return found is null ? null : ToDto(found);
    }

    public async Task<IEnumerable<LoyaltyAccountDto>> GetByPassengerAsync(
        int               passengerId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByPassenger.ExecuteAsync(passengerId, cancellationToken);
        return list.Select(ToDto);
    }

    private static LoyaltyAccountDto ToDto(LoyaltyAccountAggregate agg)
        => new(
            agg.Id.Value,
            agg.PassengerId,
            agg.LoyaltyProgramId,
            agg.LoyaltyTierId,
            agg.TotalMiles,
            agg.AvailableMiles,
            agg.JoinedAt);
}
