namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto?>             GetByIdAsync(int id,                                                                         CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentDto>> GetAllAsync(                                                                                 CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentDto>> GetByReservationAsync(int reservationId,                                                     CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentDto>> GetByTicketAsync(int ticketId,                                                               CancellationToken cancellationToken = default);
    Task<PaymentDto>              CreateForReservationAsync(int reservationId, int currencyId, decimal amount, int paymentMethodId, string? transactionReference = null, string? rejectionReason = null, CancellationToken cancellationToken = default);
    Task<PaymentDto>              CreateAsync(CreatePaymentRequest request,                                                    CancellationToken cancellationToken = default);
    Task                          UpdateStatusAsync(int id, int paymentStatusId, string? transactionReference, string? rejectionReason, CancellationToken cancellationToken = default);
    Task                          DeleteAsync(int id,                                                                          CancellationToken cancellationToken = default);
}

public sealed record PaymentDto(
    int      Id,
    int?     ReservationId,
    int?     TicketId,
    int      CurrencyId,
    DateTime PaymentDate,
    decimal  Amount,
    int      PaymentStatusId,
    int      PaymentMethodId,
    string?  TransactionReference,
    string?  RejectionReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreatePaymentRequest(
    int?    ReservationId,
    int?    TicketId,
    int     CurrencyId,
    decimal Amount,
    int     PaymentStatusId,
    int     PaymentMethodId,
    string? TransactionReference = null,
    string? RejectionReason      = null);
