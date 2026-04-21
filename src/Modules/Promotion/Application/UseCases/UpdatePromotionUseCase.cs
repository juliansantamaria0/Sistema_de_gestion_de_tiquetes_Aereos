namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdatePromotionUseCase
{
    private readonly IPromotionRepository _repository;
    private readonly IUnitOfWork          _unitOfWork;

    public UpdatePromotionUseCase(IPromotionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            name,
        decimal           discountPct,
        DateOnly          validFrom,
        DateOnly          validUntil,
        CancellationToken cancellationToken = default)
    {
        var promotion = await _repository.GetByIdAsync(new PromotionId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Promotion with id {id} was not found.");

        promotion.Update(name, discountPct, validFrom, validUntil);
        await _repository.UpdateAsync(promotion, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
