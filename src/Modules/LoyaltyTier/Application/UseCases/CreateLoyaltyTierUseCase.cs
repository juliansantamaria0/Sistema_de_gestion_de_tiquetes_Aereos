namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateLoyaltyTierUseCase
{
    private readonly ILoyaltyTierRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public CreateLoyaltyTierUseCase(ILoyaltyTierRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoyaltyTierAggregate> ExecuteAsync(
        int               loyaltyProgramId,
        string            name,
        int               minMiles,
        string?           benefits,
        CancellationToken cancellationToken = default)
    {
        
        var tier = new LoyaltyTierAggregate(
            new LoyaltyTierId(0), loyaltyProgramId, name, minMiles, benefits);

        await _repository.AddAsync(tier, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return tier;
    }
}
