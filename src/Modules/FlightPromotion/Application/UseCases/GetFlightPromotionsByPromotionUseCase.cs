namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Repositories;

/// <summary>Obtiene todos los vuelos a los que está asignada una promoción.</summary>
public sealed class GetFlightPromotionsByPromotionUseCase
{
    private readonly IFlightPromotionRepository _repository;

    public GetFlightPromotionsByPromotionUseCase(IFlightPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightPromotionAggregate>> ExecuteAsync(
        int promotionId, CancellationToken cancellationToken = default)
        => await _repository.GetByPromotionAsync(promotionId, cancellationToken);
}
