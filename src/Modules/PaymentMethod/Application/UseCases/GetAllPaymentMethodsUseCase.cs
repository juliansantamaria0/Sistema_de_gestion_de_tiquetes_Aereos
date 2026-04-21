namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Repositories;

public sealed class GetAllPaymentMethodsUseCase
{
    private readonly IPaymentMethodRepository _repository;

    public GetAllPaymentMethodsUseCase(IPaymentMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentMethodAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
