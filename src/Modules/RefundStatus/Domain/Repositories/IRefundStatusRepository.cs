namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;

public interface IRefundStatusRepository
{
    Task<RefundStatusAggregate?>             GetByIdAsync(RefundStatusId id,                 CancellationToken cancellationToken = default);
    Task<IEnumerable<RefundStatusAggregate>> GetAllAsync(                                     CancellationToken cancellationToken = default);
    Task                                     AddAsync(RefundStatusAggregate refundStatus,     CancellationToken cancellationToken = default);
    Task                                     UpdateAsync(RefundStatusAggregate refundStatus,  CancellationToken cancellationToken = default);
    Task                                     DeleteAsync(RefundStatusId id,                   CancellationToken cancellationToken = default);
}
