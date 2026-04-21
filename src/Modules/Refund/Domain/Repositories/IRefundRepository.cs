namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.ValueObject;

public interface IRefundRepository
{
    Task<RefundAggregate?>             GetByIdAsync(RefundId id,                      CancellationToken cancellationToken = default);
    Task<IEnumerable<RefundAggregate>> GetAllAsync(                                    CancellationToken cancellationToken = default);
    Task<IEnumerable<RefundAggregate>> GetByPaymentAsync(int paymentId,               CancellationToken cancellationToken = default);
    Task                               AddAsync(RefundAggregate refund,               CancellationToken cancellationToken = default);
    Task                               UpdateAsync(RefundAggregate refund,            CancellationToken cancellationToken = default);
    Task                               DeleteAsync(RefundId id,                       CancellationToken cancellationToken = default);
}
