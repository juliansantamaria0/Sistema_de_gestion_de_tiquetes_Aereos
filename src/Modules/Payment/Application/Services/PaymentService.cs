namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;

public sealed class PaymentService : IPaymentService
{
    private readonly CreatePaymentUseCase          _create;
    private readonly DeletePaymentUseCase          _delete;
    private readonly GetAllPaymentsUseCase         _getAll;
    private readonly GetPaymentByIdUseCase         _getById;
    private readonly UpdatePaymentStatusUseCase    _updateStatus;
    private readonly GetPaymentsByReservationUseCase _getByReservation;
    private readonly GetPaymentsByTicketUseCase    _getByTicket;

    public PaymentService(
        CreatePaymentUseCase           create,
        DeletePaymentUseCase           delete,
        GetAllPaymentsUseCase          getAll,
        GetPaymentByIdUseCase          getById,
        UpdatePaymentStatusUseCase     updateStatus,
        GetPaymentsByReservationUseCase getByReservation,
        GetPaymentsByTicketUseCase     getByTicket)
    {
        _create          = create;
        _delete          = delete;
        _getAll          = getAll;
        _getById         = getById;
        _updateStatus    = updateStatus;
        _getByReservation = getByReservation;
        _getByTicket     = getByTicket;
    }

    public async Task<PaymentDto> CreateAsync(
        CreatePaymentRequest request,
        CancellationToken    cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(request, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PaymentDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<PaymentDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateStatusAsync(
        int               id,
        int               paymentStatusId,
        string?           transactionReference,
        string?           rejectionReason,
        CancellationToken cancellationToken = default)
        => await _updateStatus.ExecuteAsync(id, paymentStatusId, transactionReference, rejectionReason, cancellationToken);

    public async Task<IEnumerable<PaymentDto>> GetByReservationAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByReservation.ExecuteAsync(reservationId, cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<IEnumerable<PaymentDto>> GetByTicketAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByTicket.ExecuteAsync(ticketId, cancellationToken);
        return list.Select(ToDto);
    }

    // ── Mapper privado ────────────────────────────────────────────────────────

    private static PaymentDto ToDto(PaymentAggregate agg)
        => new(
            agg.Id.Value,
            agg.ReservationId,
            agg.TicketId,
            agg.CurrencyId,
            agg.PaymentDate,
            agg.Amount,
            agg.PaymentStatusId,
            agg.PaymentMethodId,
            agg.TransactionReference,
            agg.RejectionReason,
            agg.CreatedAt,
            agg.UpdatedAt);
}
