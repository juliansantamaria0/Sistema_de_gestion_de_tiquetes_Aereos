namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.ValueObject;

public interface IPromotionRepository
{
    Task<PromotionAggregate?>             GetByIdAsync(PromotionId id,                      CancellationToken cancellationToken = default);
    Task<IEnumerable<PromotionAggregate>> GetAllAsync(                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<PromotionAggregate>> GetByAirlineAsync(int airlineId,                  CancellationToken cancellationToken = default);
    Task<IEnumerable<PromotionAggregate>> GetActiveAsync(DateOnly referenceDate,            CancellationToken cancellationToken = default);
    Task                                  AddAsync(PromotionAggregate promotion,            CancellationToken cancellationToken = default);
    Task                                  UpdateAsync(PromotionAggregate promotion,         CancellationToken cancellationToken = default);
    Task                                  DeleteAsync(PromotionId id,                       CancellationToken cancellationToken = default);
}
