namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;

public sealed class GetAllPassengerDiscountsUseCase
{
    private readonly IPassengerDiscountRepository _repository;

    public GetAllPassengerDiscountsUseCase(IPassengerDiscountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PassengerDiscountAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
