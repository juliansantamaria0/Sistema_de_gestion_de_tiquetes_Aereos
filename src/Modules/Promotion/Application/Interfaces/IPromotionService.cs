namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.Interfaces;

public interface IPromotionService
{
    Task<PromotionDto?>             GetByIdAsync(int id,                                                                                    CancellationToken cancellationToken = default);
    Task<IEnumerable<PromotionDto>> GetAllAsync(                                                                                            CancellationToken cancellationToken = default);
    Task<IEnumerable<PromotionDto>> GetByAirlineAsync(int airlineId,                                                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<PromotionDto>> GetActiveAsync(DateOnly referenceDate,                                                                 CancellationToken cancellationToken = default);
    Task<PromotionDto>              CreateAsync(int airlineId, string name, decimal discountPct, DateOnly validFrom, DateOnly validUntil,   CancellationToken cancellationToken = default);
    Task                            UpdateAsync(int id, string name, decimal discountPct, DateOnly validFrom, DateOnly validUntil,          CancellationToken cancellationToken = default);
    Task                            DeleteAsync(int id,                                                                                    CancellationToken cancellationToken = default);
}

public sealed record PromotionDto(
    int     Id,
    int     AirlineId,
    string  Name,
    decimal DiscountPct,
    DateOnly ValidFrom,
    DateOnly ValidUntil);
