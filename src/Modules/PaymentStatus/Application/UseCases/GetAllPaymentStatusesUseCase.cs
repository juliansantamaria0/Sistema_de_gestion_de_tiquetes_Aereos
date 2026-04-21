namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Repositories;

public sealed class GetAllPaymentStatusesUseCase
{
    private readonly IPaymentStatusRepository _repository;

    public GetAllPaymentStatusesUseCase(IPaymentStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentStatusAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
