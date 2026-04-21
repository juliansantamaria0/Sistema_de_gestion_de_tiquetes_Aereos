namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;

public interface IReservationRepository
{
    Task<ReservationAggregate?>             GetByIdAsync(ReservationId id,                     CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationAggregate>> GetAllAsync(                                        CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationAggregate>> GetByCustomerAsync(int customerId,                  CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationAggregate>> GetByFlightAsync(int scheduledFlightId,             CancellationToken cancellationToken = default);
    Task                                    AddAsync(ReservationAggregate reservation,          CancellationToken cancellationToken = default);
    Task                                    UpdateAsync(ReservationAggregate reservation,       CancellationToken cancellationToken = default);
    Task                                    DeleteAsync(ReservationId id,                       CancellationToken cancellationToken = default);
}
