namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Aggregate;

public sealed class DiscountTypeService : IDiscountTypeService
{
    private readonly CreateDiscountTypeUseCase  _create;
    private readonly DeleteDiscountTypeUseCase  _delete;
    private readonly GetAllDiscountTypesUseCase _getAll;
    private readonly GetDiscountTypeByIdUseCase _getById;
    private readonly UpdateDiscountTypeUseCase  _update;

    public DiscountTypeService(
        CreateDiscountTypeUseCase  create,
        DeleteDiscountTypeUseCase  delete,
        GetAllDiscountTypesUseCase getAll,
        GetDiscountTypeByIdUseCase getById,
        UpdateDiscountTypeUseCase  update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<DiscountTypeDto> CreateAsync(
        string name, decimal percentage, int? ageMin, int? ageMax,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, percentage, ageMin, ageMax, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<DiscountTypeDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<DiscountTypeDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int id, string name, decimal percentage, int? ageMin, int? ageMax,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, percentage, ageMin, ageMax, cancellationToken);

    private static DiscountTypeDto ToDto(DiscountTypeAggregate agg)
        => new(agg.Id.Value, agg.Name, agg.Percentage, agg.AgeMin, agg.AgeMax);
}
