namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;

public sealed class GetAllPromotionsUseCase
{
    private readonly IPromotionRepository _repository;

    public GetAllPromotionsUseCase(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PromotionAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
