namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Repositories;

/// <summary>Obtiene todos los reembolsos asociados a un pago.</summary>
public sealed class GetRefundsByPaymentUseCase
{
    private readonly IRefundRepository _repository;

    public GetRefundsByPaymentUseCase(IRefundRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RefundAggregate>> ExecuteAsync(
        int               paymentId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByPaymentAsync(paymentId, cancellationToken);
}
