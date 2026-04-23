namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePromotionUseCase
{
    private readonly IPromotionRepository _repository;
    private readonly IUnitOfWork          _unitOfWork;

    public CreatePromotionUseCase(IPromotionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PromotionAggregate> ExecuteAsync(
        int               airlineId,
        string            name,
        decimal           discountPct,
        DateOnly          validFrom,
        DateOnly          validUntil,
        CancellationToken cancellationToken = default)
    {
        var promotion = new PromotionAggregate(
            new PromotionId(0), airlineId, name, discountPct, validFrom, validUntil);

        await _repository.AddAsync(promotion, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return promotion;
    }
}
