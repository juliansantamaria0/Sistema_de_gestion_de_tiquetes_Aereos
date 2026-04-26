namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Services;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class PaymentService : IPaymentService
{
    private readonly CreatePaymentUseCase            _create;
    private readonly DeletePaymentUseCase            _delete;
    private readonly GetAllPaymentsUseCase           _getAll;
    private readonly GetPaymentByIdUseCase           _getById;
    private readonly UpdatePaymentStatusUseCase      _updateStatus;
    private readonly GetPaymentsByReservationUseCase _getByReservation;
    private readonly GetPaymentsByTicketUseCase      _getByTicket;
    private readonly AppDbContext                    _db;

    public PaymentService(
        CreatePaymentUseCase            create,
        DeletePaymentUseCase            delete,
        GetAllPaymentsUseCase           getAll,
        GetPaymentByIdUseCase           getById,
        UpdatePaymentStatusUseCase      updateStatus,
        GetPaymentsByReservationUseCase getByReservation,
        GetPaymentsByTicketUseCase      getByTicket,
        AppDbContext                    db)
    {
        _create           = create;
        _delete           = delete;
        _getAll           = getAll;
        _getById          = getById;
        _updateStatus     = updateStatus;
        _getByReservation = getByReservation;
        _getByTicket      = getByTicket;
        _db               = db;
    }

    public async Task<PaymentDto> CreateAsync(
        CreatePaymentRequest request,
        CancellationToken    cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(request, cancellationToken);
        return ToDto(agg);
    }

    public async Task<PaymentDto> CreateForReservationAsync(
        int               reservationId,
        int               currencyId,
        decimal           amount,
        int               paymentMethodId,
        string?           transactionReference = null,
        string?           rejectionReason      = null,
        CancellationToken cancellationToken    = default)
    {
        var agg = await _create.ExecuteAsync(
            new CreatePaymentRequest(
                ReservationId: reservationId,
                TicketId: null,
                CurrencyId: currencyId,
                Amount: amount,
                PaymentStatusId: 0,
                PaymentMethodId: paymentMethodId,
                TransactionReference: transactionReference,
                RejectionReason: rejectionReason),
            cancellationToken);

        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PaymentDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var customerId = CurrentUser.CustomerId.Value;
            var reservationIds = await _db.Reservations
                .AsNoTracking()
                .Where(r => r.CustomerId == customerId)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            var results = new List<PaymentDto>();
            foreach (var rid in reservationIds)
            {
                var payments = await _getByReservation.ExecuteAsync(rid, cancellationToken);
                results.AddRange(payments.Select(ToDto));
            }
            return results;
        }
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
