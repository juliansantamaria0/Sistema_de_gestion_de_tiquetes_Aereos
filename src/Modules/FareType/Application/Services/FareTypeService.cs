namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Aggregate;

public sealed class FareTypeService : IFareTypeService
{
    private readonly CreateFareTypeUseCase   _create;
    private readonly DeleteFareTypeUseCase   _delete;
    private readonly GetAllFareTypesUseCase  _getAll;
    private readonly GetFareTypeByIdUseCase  _getById;
    private readonly UpdateFareTypeUseCase   _update;

    public FareTypeService(
        CreateFareTypeUseCase  create,
        DeleteFareTypeUseCase  delete,
        GetAllFareTypesUseCase getAll,
        GetFareTypeByIdUseCase getById,
        UpdateFareTypeUseCase  update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<FareTypeDto> CreateAsync(
        string name, bool isRefundable, bool isChangeable,
        int advancePurchaseDays, bool baggageIncluded,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            name, isRefundable, isChangeable, advancePurchaseDays, baggageIncluded,
            cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FareTypeDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FareTypeDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int id, string name, bool isRefundable, bool isChangeable,
        int advancePurchaseDays, bool baggageIncluded,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(
            id, name, isRefundable, isChangeable, advancePurchaseDays, baggageIncluded,
            cancellationToken);

    private static FareTypeDto ToDto(FareTypeAggregate agg)
        => new(agg.Id.Value, agg.Name, agg.IsRefundable, agg.IsChangeable,
               agg.AdvancePurchaseDays, agg.BaggageIncluded);
}
