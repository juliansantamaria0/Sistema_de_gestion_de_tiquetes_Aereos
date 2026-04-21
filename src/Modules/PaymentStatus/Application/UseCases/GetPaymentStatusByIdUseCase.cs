namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.ValueObject;

public sealed class GetPaymentStatusByIdUseCase
{
    private readonly IPaymentStatusRepository _repository;

    public GetPaymentStatusByIdUseCase(IPaymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentStatusAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PaymentStatusId(id), cancellationToken);
}
