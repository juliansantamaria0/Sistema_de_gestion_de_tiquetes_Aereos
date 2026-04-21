namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;

public interface IPaymentRepository
{
    Task<PaymentAggregate?>             GetByIdAsync(PaymentId id,                         CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentAggregate>> GetAllAsync(                                        CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentAggregate>> GetByReservationAsync(int reservationId,            CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentAggregate>> GetByTicketAsync(int ticketId,                      CancellationToken cancellationToken = default);
    Task                                AddAsync(PaymentAggregate payment,                  CancellationToken cancellationToken = default);
    Task                                UpdateAsync(PaymentAggregate payment,               CancellationToken cancellationToken = default);
    Task                                DeleteAsync(PaymentId id,                           CancellationToken cancellationToken = default);
}
