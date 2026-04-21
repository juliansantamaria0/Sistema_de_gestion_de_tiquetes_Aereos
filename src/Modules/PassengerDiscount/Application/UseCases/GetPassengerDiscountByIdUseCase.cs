namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;

public sealed class GetPassengerDiscountByIdUseCase
{
    private readonly IPassengerDiscountRepository _repository;

    public GetPassengerDiscountByIdUseCase(IPassengerDiscountRepository repository)
    {
        _repository = repository;
    }

    public async Task<PassengerDiscountAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PassengerDiscountId(id), cancellationToken);
}
