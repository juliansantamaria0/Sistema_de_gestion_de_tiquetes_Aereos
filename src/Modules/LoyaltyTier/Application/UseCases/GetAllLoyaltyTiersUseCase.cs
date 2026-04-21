namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Repositories;

public sealed class GetAllLoyaltyTiersUseCase
{
    private readonly ILoyaltyTierRepository _repository;

    public GetAllLoyaltyTiersUseCase(ILoyaltyTierRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LoyaltyTierAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
