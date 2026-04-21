namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;

public interface IReservationStatusRepository
{
    Task<ReservationStatusAggregate?>             GetByIdAsync(ReservationStatusId id,                    CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationStatusAggregate>> GetAllAsync(                                             CancellationToken cancellationToken = default);
    Task                                          AddAsync(ReservationStatusAggregate reservationStatus,   CancellationToken cancellationToken = default);
    Task                                          UpdateAsync(ReservationStatusAggregate reservationStatus,CancellationToken cancellationToken = default);
    Task                                          DeleteAsync(ReservationStatusId id,                      CancellationToken cancellationToken = default);
}
