namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Repositories;

/// <summary>
/// Obtiene todos los niveles (tiers) de un programa de fidelización,
/// ordenados por millas mínimas ascendente (Classic → Diamond).
/// Caso de uso clave para mostrar la estructura de un programa.
/// </summary>
public sealed class GetLoyaltyTiersByProgramUseCase
{
    private readonly ILoyaltyTierRepository _repository;

    public GetLoyaltyTiersByProgramUseCase(ILoyaltyTierRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LoyaltyTierAggregate>> ExecuteAsync(
        int               loyaltyProgramId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByProgramAsync(loyaltyProgramId, cancellationToken);
}
