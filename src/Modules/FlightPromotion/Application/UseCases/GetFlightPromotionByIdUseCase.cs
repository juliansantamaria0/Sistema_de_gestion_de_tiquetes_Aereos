namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.ValueObject;

public sealed class GetFlightPromotionByIdUseCase
{
    private readonly IFlightPromotionRepository _repository;

    public GetFlightPromotionByIdUseCase(IFlightPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightPromotionAggregate?> ExecuteAsync(
        int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightPromotionId(id), cancellationToken);
}
