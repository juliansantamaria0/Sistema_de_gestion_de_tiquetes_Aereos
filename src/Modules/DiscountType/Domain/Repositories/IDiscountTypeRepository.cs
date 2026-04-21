namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;

public interface IDiscountTypeRepository
{
    Task<DiscountTypeAggregate?>             GetByIdAsync(DiscountTypeId id,                 CancellationToken cancellationToken = default);
    Task<IEnumerable<DiscountTypeAggregate>> GetAllAsync(                                     CancellationToken cancellationToken = default);
    Task                                     AddAsync(DiscountTypeAggregate discountType,    CancellationToken cancellationToken = default);
    Task                                     UpdateAsync(DiscountTypeAggregate discountType, CancellationToken cancellationToken = default);
    Task                                     DeleteAsync(DiscountTypeId id,                  CancellationToken cancellationToken = default);
}
