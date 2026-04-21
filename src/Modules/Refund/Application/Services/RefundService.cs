namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;

public sealed class RefundService : IRefundService
{
    private readonly CreateRefundUseCase        _create;
    private readonly DeleteRefundUseCase        _delete;
    private readonly GetAllRefundsUseCase       _getAll;
    private readonly GetRefundByIdUseCase       _getById;
    private readonly UpdateRefundStatusUseCase  _updateStatus;
    private readonly GetRefundsByPaymentUseCase _getByPayment;

    public RefundService(
        CreateRefundUseCase        create,
        DeleteRefundUseCase        delete,
        GetAllRefundsUseCase       getAll,
        GetRefundByIdUseCase       getById,
        UpdateRefundStatusUseCase  updateStatus,
        GetRefundsByPaymentUseCase getByPayment)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _updateStatus = updateStatus;
        _getByPayment = getByPayment;
    }

    public async Task<RefundDto> CreateAsync(
        int               paymentId,
        int               refundStatusId,
        decimal           amount,
        string?           reason,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            paymentId, refundStatusId, amount, reason, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<RefundDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<RefundDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateStatusAsync(
        int               id,
        int               refundStatusId,
        DateTime?         processedAt,
        string?           reason,
        CancellationToken cancellationToken = default)
        => await _updateStatus.ExecuteAsync(id, refundStatusId, processedAt, reason, cancellationToken);

    public async Task<IEnumerable<RefundDto>> GetByPaymentAsync(
        int               paymentId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByPayment.ExecuteAsync(paymentId, cancellationToken);
        return list.Select(ToDto);
    }

    // ── Mapper privado ────────────────────────────────────────────────────────

    private static RefundDto ToDto(RefundAggregate agg)
        => new(
            agg.Id.Value,
            agg.PaymentId,
            agg.RefundStatusId,
            agg.Amount,
            agg.RequestedAt,
            agg.ProcessedAt,
            agg.Reason);
}
