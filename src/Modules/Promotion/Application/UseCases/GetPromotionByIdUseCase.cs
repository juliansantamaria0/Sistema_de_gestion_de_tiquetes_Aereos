namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.ValueObject;

public sealed class GetPromotionByIdUseCase
{
    private readonly IPromotionRepository _repository;

    public GetPromotionByIdUseCase(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PromotionAggregate?> ExecuteAsync(
        int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PromotionId(id), cancellationToken);
}
