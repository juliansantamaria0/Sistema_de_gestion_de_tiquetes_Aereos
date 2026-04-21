namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.ValueObject;

public interface IPaymentStatusRepository
{
    Task<PaymentStatusAggregate?>             GetByIdAsync(PaymentStatusId id,                CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentStatusAggregate>> GetAllAsync(                                     CancellationToken cancellationToken = default);
    Task                                      AddAsync(PaymentStatusAggregate paymentStatus,   CancellationToken cancellationToken = default);
    Task                                      UpdateAsync(PaymentStatusAggregate paymentStatus,CancellationToken cancellationToken = default);
    Task                                      DeleteAsync(PaymentStatusId id,                  CancellationToken cancellationToken = default);
}
