namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;

public sealed class GetAllPaymentsUseCase
{
    private readonly IPaymentRepository _repository;

    public GetAllPaymentsUseCase(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
