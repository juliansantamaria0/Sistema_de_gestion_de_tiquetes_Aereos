namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;

public sealed class GetLoyaltyTierByIdUseCase
{
    private readonly ILoyaltyTierRepository _repository;

    public GetLoyaltyTierByIdUseCase(ILoyaltyTierRepository repository)
    {
        _repository = repository;
    }

    public async Task<LoyaltyTierAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new LoyaltyTierId(id), cancellationToken);
}
