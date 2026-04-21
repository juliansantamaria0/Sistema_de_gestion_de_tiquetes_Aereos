namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;

public sealed class PromotionService : IPromotionService
{
    private readonly CreatePromotionUseCase        _create;
    private readonly DeletePromotionUseCase        _delete;
    private readonly GetAllPromotionsUseCase       _getAll;
    private readonly GetPromotionByIdUseCase       _getById;
    private readonly UpdatePromotionUseCase        _update;
    private readonly GetPromotionsByAirlineUseCase _getByAirline;
    private readonly GetActivePromotionsUseCase    _getActive;

    public PromotionService(
        CreatePromotionUseCase        create,
        DeletePromotionUseCase        delete,
        GetAllPromotionsUseCase       getAll,
        GetPromotionByIdUseCase       getById,
        UpdatePromotionUseCase        update,
        GetPromotionsByAirlineUseCase getByAirline,
        GetActivePromotionsUseCase    getActive)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _update       = update;
        _getByAirline = getByAirline;
        _getActive    = getActive;
    }

    public async Task<PromotionDto> CreateAsync(
        int airlineId, string name, decimal discountPct,
        DateOnly validFrom, DateOnly validUntil,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            airlineId, name, discountPct, validFrom, validUntil, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PromotionDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<PromotionDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int id, string name, decimal discountPct,
        DateOnly validFrom, DateOnly validUntil,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, discountPct, validFrom, validUntil, cancellationToken);

    public async Task<IEnumerable<PromotionDto>> GetByAirlineAsync(
        int airlineId, CancellationToken cancellationToken = default)
    {
        var list = await _getByAirline.ExecuteAsync(airlineId, cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<IEnumerable<PromotionDto>> GetActiveAsync(
        DateOnly referenceDate, CancellationToken cancellationToken = default)
    {
        var list = await _getActive.ExecuteAsync(referenceDate, cancellationToken);
        return list.Select(ToDto);
    }

    private static PromotionDto ToDto(PromotionAggregate agg)
        => new(agg.Id.Value, agg.AirlineId, agg.Name,
               agg.DiscountPct, agg.ValidFrom, agg.ValidUntil);
}
