namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.ValueObject;

public interface IFlightPromotionRepository
{
    Task<FlightPromotionAggregate?>             GetByIdAsync(FlightPromotionId id,               CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightPromotionAggregate>> GetAllAsync(                                      CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightPromotionAggregate>> GetByFlightAsync(int scheduledFlightId,          CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightPromotionAggregate>> GetByPromotionAsync(int promotionId,             CancellationToken cancellationToken = default);
    Task                                        AddAsync(FlightPromotionAggregate flightPromotion, CancellationToken cancellationToken = default);
    Task                                        DeleteAsync(FlightPromotionId id,                CancellationToken cancellationToken = default);
    
}
