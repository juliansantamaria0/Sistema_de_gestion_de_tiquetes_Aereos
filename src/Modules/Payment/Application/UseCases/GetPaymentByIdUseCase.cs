namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;

public sealed class GetPaymentByIdUseCase
{
    private readonly IPaymentRepository _repository;

    public GetPaymentByIdUseCase(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PaymentId(id), cancellationToken);
}
