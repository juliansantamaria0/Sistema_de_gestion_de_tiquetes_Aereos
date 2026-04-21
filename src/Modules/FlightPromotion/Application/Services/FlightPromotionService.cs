namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;

public sealed class FlightPromotionService : IFlightPromotionService
{
    private readonly AssignFlightPromotionUseCase           _assign;
    private readonly RemoveFlightPromotionUseCase           _remove;
    private readonly GetAllFlightPromotionsUseCase          _getAll;
    private readonly GetFlightPromotionByIdUseCase          _getById;
    private readonly GetFlightPromotionsByFlightUseCase     _getByFlight;
    private readonly GetFlightPromotionsByPromotionUseCase  _getByPromotion;

    public FlightPromotionService(
        AssignFlightPromotionUseCase          assign,
        RemoveFlightPromotionUseCase          remove,
        GetAllFlightPromotionsUseCase         getAll,
        GetFlightPromotionByIdUseCase         getById,
        GetFlightPromotionsByFlightUseCase    getByFlight,
        GetFlightPromotionsByPromotionUseCase getByPromotion)
    {
        _assign         = assign;
        _remove         = remove;
        _getAll         = getAll;
        _getById        = getById;
        _getByFlight    = getByFlight;
        _getByPromotion = getByPromotion;
    }

    public async Task<FlightPromotionDto> AssignAsync(
        int scheduledFlightId, int promotionId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _assign.ExecuteAsync(scheduledFlightId, promotionId, cancellationToken);
        return ToDto(agg);
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
        => await _remove.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightPromotionDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FlightPromotionDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task<IEnumerable<FlightPromotionDto>> GetByFlightAsync(
        int scheduledFlightId, CancellationToken cancellationToken = default)
    {
        var list = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<IEnumerable<FlightPromotionDto>> GetByPromotionAsync(
        int promotionId, CancellationToken cancellationToken = default)
    {
        var list = await _getByPromotion.ExecuteAsync(promotionId, cancellationToken);
        return list.Select(ToDto);
    }

    private static FlightPromotionDto ToDto(FlightPromotionAggregate agg)
        => new(agg.Id.Value, agg.ScheduledFlightId, agg.PromotionId);
}
