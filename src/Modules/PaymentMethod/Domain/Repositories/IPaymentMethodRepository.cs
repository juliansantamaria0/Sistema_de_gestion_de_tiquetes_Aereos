namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;

public interface IPaymentMethodRepository
{
    Task<PaymentMethodAggregate?>             GetByIdAsync(PaymentMethodId id,                 CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentMethodAggregate>> GetAllAsync(                                      CancellationToken cancellationToken = default);
    Task                                      AddAsync(PaymentMethodAggregate paymentMethod,    CancellationToken cancellationToken = default);
    Task                                      UpdateAsync(PaymentMethodAggregate paymentMethod, CancellationToken cancellationToken = default);
    Task                                      DeleteAsync(PaymentMethodId id,                   CancellationToken cancellationToken = default);
}
