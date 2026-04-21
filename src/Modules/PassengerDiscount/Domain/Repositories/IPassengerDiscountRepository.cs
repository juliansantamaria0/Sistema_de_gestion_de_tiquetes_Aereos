namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;

public interface IPassengerDiscountRepository
{
    Task<PassengerDiscountAggregate?>             GetByIdAsync(PassengerDiscountId id,                      CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerDiscountAggregate>> GetAllAsync(                                               CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerDiscountAggregate>> GetByReservationDetailAsync(int reservationDetailId,       CancellationToken cancellationToken = default);
    Task                                          AddAsync(PassengerDiscountAggregate passengerDiscount,     CancellationToken cancellationToken = default);
    Task                                          UpdateAsync(PassengerDiscountAggregate passengerDiscount,  CancellationToken cancellationToken = default);
    Task                                          DeleteAsync(PassengerDiscountId id,                        CancellationToken cancellationToken = default);
}
