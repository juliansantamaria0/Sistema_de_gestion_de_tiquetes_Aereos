namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;

/// <summary>Obtiene todas las promociones de una aerolínea.</summary>
public sealed class GetPromotionsByAirlineUseCase
{
    private readonly IPromotionRepository _repository;

    public GetPromotionsByAirlineUseCase(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PromotionAggregate>> ExecuteAsync(
        int airlineId, CancellationToken cancellationToken = default)
        => await _repository.GetByAirlineAsync(airlineId, cancellationToken);
}
