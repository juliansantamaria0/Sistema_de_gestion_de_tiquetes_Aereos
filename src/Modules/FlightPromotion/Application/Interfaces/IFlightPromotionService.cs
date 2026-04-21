namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.Interfaces;

public interface IFlightPromotionService
{
    Task<FlightPromotionDto?>             GetByIdAsync(int id,                                                CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightPromotionDto>> GetAllAsync(                                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightPromotionDto>> GetByFlightAsync(int scheduledFlightId,                           CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightPromotionDto>> GetByPromotionAsync(int promotionId,                              CancellationToken cancellationToken = default);
    Task<FlightPromotionDto>              AssignAsync(int scheduledFlightId, int promotionId,               CancellationToken cancellationToken = default);
    Task                                  RemoveAsync(int id,                                               CancellationToken cancellationToken = default);
}

public sealed record FlightPromotionDto(
    int Id,
    int ScheduledFlightId,
    int PromotionId);
