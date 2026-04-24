namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static PaymentAggregate ToDomain(PaymentEntity entity)
        => new(
            new PaymentId(entity.Id),
            entity.ReservationId,
            entity.TicketId,
            entity.CurrencyId,
            entity.PaymentDate,
            entity.Amount,
            entity.PaymentStatusId,
            entity.PaymentMethodId,
            entity.TransactionReference,
            entity.RejectionReason,
            entity.CreatedAt,
            entity.UpdatedAt);

    

    public async Task<PaymentAggregate?> GetByIdAsync(
        PaymentId         id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<PaymentAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Payments
            .AsNoTracking()
            .OrderByDescending(e => e.PaymentDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<PaymentAggregate>> GetByReservationAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Payments
            .AsNoTracking()
            .Where(e => e.ReservationId == reservationId)
            .OrderByDescending(e => e.PaymentDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<PaymentAggregate>> GetByTicketAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Payments
            .AsNoTracking()
            .Where(e => e.TicketId == ticketId)
            .OrderByDescending(e => e.PaymentDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        PaymentAggregate  payment,
        CancellationToken cancellationToken = default)
    {
        var entity = new PaymentEntity
        {
            ReservationId        = payment.ReservationId,
            TicketId             = payment.TicketId,
            CurrencyId           = payment.CurrencyId,
            PaymentDate          = payment.PaymentDate,
            Amount               = payment.Amount,
            PaymentStatusId      = payment.PaymentStatusId,
            PaymentMethodId      = payment.PaymentMethodId,
            TransactionReference = payment.TransactionReference,
            RejectionReason      = payment.RejectionReason,
            CreatedAt            = payment.CreatedAt,
            UpdatedAt            = payment.UpdatedAt
        };
        await _context.Payments.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        PaymentAggregate  payment,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Payments
            .FirstOrDefaultAsync(e => e.Id == payment.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PaymentEntity with id {payment.Id.Value} not found.");

        
        
        entity.PaymentStatusId      = payment.PaymentStatusId;
        entity.TransactionReference = payment.TransactionReference;
        entity.RejectionReason      = payment.RejectionReason;
        entity.UpdatedAt            = payment.UpdatedAt;

        _context.Payments.Update(entity);
    }

    public async Task DeleteAsync(
        PaymentId         id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Payments
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PaymentEntity with id {id.Value} not found.");

        _context.Payments.Remove(entity);
    }

    public async Task<decimal> SumApprovedPaymentsForReservationAsync(
        int reservationId,
        CancellationToken cancellationToken = default)
    {
        return await (
                from p in _context.Payments.AsNoTracking()
                where p.ReservationId == reservationId
                join s in _context.PaymentStatuses.AsNoTracking() on p.PaymentStatusId equals s.Id
                // MySQL EF provider can't translate string.Equals(x, StringComparison.*).
                // Use ToUpper() which is translatable.
                where s.Name != null
                      && (s.Name.ToUpper() == "PAID" || s.Name.ToUpper() == "APROBADO")
                select (decimal?)p.Amount)
            .SumAsync(cancellationToken) ?? 0m;
    }
}
