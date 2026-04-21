namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Repositories;

public sealed class GetAllFlightPromotionsUseCase
{
    private readonly IFlightPromotionRepository _repository;

    public GetAllFlightPromotionsUseCase(IFlightPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightPromotionAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
