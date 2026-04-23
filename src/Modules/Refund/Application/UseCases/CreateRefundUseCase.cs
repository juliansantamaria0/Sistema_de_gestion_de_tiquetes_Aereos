namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateRefundUseCase
{
    private readonly IRefundRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public CreateRefundUseCase(IRefundRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RefundAggregate> ExecuteAsync(
        int               paymentId,
        int               refundStatusId,
        decimal           amount,
        string?           reason,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // RefundId(1) es placeholder; EF Core asigna el Id real al insertar.
        var refund = new RefundAggregate(
            new RefundId(await GetNextIdAsync(cancellationToken)),
            paymentId,
            refundStatusId,
            amount,
            now,
            processedAt: null,
            reason: reason);

        await _repository.AddAsync(refund, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return refund;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
