namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;

public sealed class GetDiscountTypeByIdUseCase
{
    private readonly IDiscountTypeRepository _repository;

    public GetDiscountTypeByIdUseCase(IDiscountTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<DiscountTypeAggregate?> ExecuteAsync(
        int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new DiscountTypeId(id), cancellationToken);
}
