namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Repositories;

public sealed class GetAllDiscountTypesUseCase
{
    private readonly IDiscountTypeRepository _repository;

    public GetAllDiscountTypesUseCase(IDiscountTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DiscountTypeAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
