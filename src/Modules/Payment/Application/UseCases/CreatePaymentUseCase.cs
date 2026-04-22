namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePaymentUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentAggregate> ExecuteAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        if (request.Amount <= 0)
            throw new InvalidOperationException("El monto del pago debe ser mayor que cero.");

        if (request.ReservationId.HasValue && request.TicketId.HasValue)
            throw new InvalidOperationException("El pago no puede asociarse simultáneamente a una reserva y a un tiquete.");
        if (!request.ReservationId.HasValue && !request.TicketId.HasValue)
            throw new InvalidOperationException("El pago debe asociarse a una reserva o a un tiquete.");

        if (!await _context.Currencies.AsNoTracking().AnyAsync(x => x.Id == request.CurrencyId, cancellationToken))
            throw new InvalidOperationException($"No existe la moneda con id {request.CurrencyId}.");
        if (!await _context.PaymentStatuses.AsNoTracking().AnyAsync(x => x.Id == request.PaymentStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de pago con id {request.PaymentStatusId}.");
        if (!await _context.PaymentMethods.AsNoTracking().AnyAsync(x => x.Id == request.PaymentMethodId, cancellationToken))
            throw new InvalidOperationException($"No existe el método de pago con id {request.PaymentMethodId}.");

        if (request.ReservationId.HasValue)
        {
            var reservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ReservationId.Value, cancellationToken)
                ?? throw new InvalidOperationException($"No existe la reserva con id {request.ReservationId.Value}.");

            if (reservation.CancelledAt.HasValue)
                throw new InvalidOperationException("No se puede registrar un pago sobre una reserva cancelada.");
        }

        if (request.TicketId.HasValue)
        {
            var ticket = await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.TicketId.Value, cancellationToken)
                ?? throw new InvalidOperationException($"No existe el tiquete con id {request.TicketId.Value}.");

            var detail = await _context.ReservationDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == ticket.ReservationDetailId, cancellationToken)
                ?? throw new InvalidOperationException("El tiquete está asociado a un detalle de reserva inexistente.");

            var reservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == detail.ReservationId, cancellationToken)
                ?? throw new InvalidOperationException("El tiquete está asociado a una reserva inexistente.");

            if (reservation.CancelledAt.HasValue)
                throw new InvalidOperationException("No se puede registrar un pago sobre un tiquete asociado a una reserva cancelada.");
        }

        var paymentEntity = new PaymentEntity
        {
            ReservationId = request.ReservationId,
            TicketId = request.TicketId,
            CurrencyId = request.CurrencyId,
            PaymentDate = now,
            Amount = request.Amount,
            PaymentStatusId = request.PaymentStatusId,
            PaymentMethodId = request.PaymentMethodId,
            TransactionReference = request.TransactionReference?.Trim(),
            RejectionReason = request.RejectionReason?.Trim(),
            CreatedAt = now,
            UpdatedAt = null
        };

        await _context.Payments.AddAsync(paymentEntity, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new PaymentAggregate(
            new PaymentId(paymentEntity.Id),
            paymentEntity.ReservationId,
            paymentEntity.TicketId,
            paymentEntity.CurrencyId,
            paymentEntity.PaymentDate,
            paymentEntity.Amount,
            paymentEntity.PaymentStatusId,
            paymentEntity.PaymentMethodId,
            paymentEntity.TransactionReference,
            paymentEntity.RejectionReason,
            paymentEntity.CreatedAt,
            paymentEntity.UpdatedAt);
    }
}
