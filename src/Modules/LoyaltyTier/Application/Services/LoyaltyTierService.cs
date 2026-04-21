namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;

public sealed class LoyaltyTierService : ILoyaltyTierService
{
    private readonly CreateLoyaltyTierUseCase         _create;
    private readonly DeleteLoyaltyTierUseCase         _delete;
    private readonly GetAllLoyaltyTiersUseCase        _getAll;
    private readonly GetLoyaltyTierByIdUseCase        _getById;
    private readonly UpdateLoyaltyTierUseCase         _update;
    private readonly GetLoyaltyTiersByProgramUseCase  _getByProgram;

    public LoyaltyTierService(
        CreateLoyaltyTierUseCase        create,
        DeleteLoyaltyTierUseCase        delete,
        GetAllLoyaltyTiersUseCase       getAll,
        GetLoyaltyTierByIdUseCase       getById,
        UpdateLoyaltyTierUseCase        update,
        GetLoyaltyTiersByProgramUseCase getByProgram)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _update       = update;
        _getByProgram = getByProgram;
    }

    public async Task<LoyaltyTierDto> CreateAsync(
        int               loyaltyProgramId,
        string            name,
        int               minMiles,
        string?           benefits,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            loyaltyProgramId, name, minMiles, benefits, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<LoyaltyTierDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<LoyaltyTierDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        int               minMiles,
        string?           benefits,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, minMiles, benefits, cancellationToken);

    public async Task<IEnumerable<LoyaltyTierDto>> GetByProgramAsync(
        int               loyaltyProgramId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByProgram.ExecuteAsync(loyaltyProgramId, cancellationToken);
        return list.Select(ToDto);
    }

    private static LoyaltyTierDto ToDto(LoyaltyTierAggregate agg)
        => new(agg.Id.Value, agg.LoyaltyProgramId, agg.Name, agg.MinMiles, agg.Benefits);
}
