namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;






public sealed class GetActivePromotionsUseCase
{
    private readonly IPromotionRepository _repository;

    public GetActivePromotionsUseCase(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PromotionAggregate>> ExecuteAsync(
        DateOnly referenceDate, CancellationToken cancellationToken = default)
        => await _repository.GetActiveAsync(referenceDate, cancellationToken);
}
