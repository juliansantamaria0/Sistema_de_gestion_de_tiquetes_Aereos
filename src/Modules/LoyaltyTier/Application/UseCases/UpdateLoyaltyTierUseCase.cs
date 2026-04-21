namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateLoyaltyTierUseCase
{
    private readonly ILoyaltyTierRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public UpdateLoyaltyTierUseCase(ILoyaltyTierRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            name,
        int               minMiles,
        string?           benefits,
        CancellationToken cancellationToken = default)
    {
        var tier = await _repository.GetByIdAsync(new LoyaltyTierId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"LoyaltyTier with id {id} was not found.");

        tier.Update(name, minMiles, benefits);
        await _repository.UpdateAsync(tier, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
