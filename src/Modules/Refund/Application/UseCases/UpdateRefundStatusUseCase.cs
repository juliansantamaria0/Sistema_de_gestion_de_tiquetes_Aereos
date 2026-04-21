namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Actualiza el estado del reembolso y opcionalmente registra
/// la fecha de procesamiento y el motivo.
/// payment_id, amount y requested_at son inmutables.
/// </summary>
public sealed class UpdateRefundStatusUseCase
{
    private readonly IRefundRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public UpdateRefundStatusUseCase(IRefundRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               refundStatusId,
        DateTime?         processedAt,
        string?           reason,
        CancellationToken cancellationToken = default)
    {
        var refund = await _repository.GetByIdAsync(new RefundId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Refund with id {id} was not found.");

        refund.UpdateStatus(refundStatusId, processedAt, reason);
        await _repository.UpdateAsync(refund, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
