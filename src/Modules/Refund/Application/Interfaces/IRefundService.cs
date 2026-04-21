namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.Interfaces;

public interface IRefundService
{
    Task<RefundDto?>             GetByIdAsync(int id,                                                                     CancellationToken cancellationToken = default);
    Task<IEnumerable<RefundDto>> GetAllAsync(                                                                             CancellationToken cancellationToken = default);
    Task<IEnumerable<RefundDto>> GetByPaymentAsync(int paymentId,                                                         CancellationToken cancellationToken = default);
    Task<RefundDto>              CreateAsync(int paymentId, int refundStatusId, decimal amount, string? reason,           CancellationToken cancellationToken = default);
    Task                         UpdateStatusAsync(int id, int refundStatusId, DateTime? processedAt, string? reason,    CancellationToken cancellationToken = default);
    Task                         DeleteAsync(int id,                                                                      CancellationToken cancellationToken = default);
}

public sealed record RefundDto(
    int      Id,
    int      PaymentId,
    int      RefundStatusId,
    decimal  Amount,
    DateTime RequestedAt,
    DateTime? ProcessedAt,
    string?  Reason);
