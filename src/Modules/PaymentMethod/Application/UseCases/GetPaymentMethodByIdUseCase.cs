namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;

public sealed class GetPaymentMethodByIdUseCase
{
    private readonly IPaymentMethodRepository _repository;

    public GetPaymentMethodByIdUseCase(IPaymentMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentMethodAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PaymentMethodId(id), cancellationToken);
}
