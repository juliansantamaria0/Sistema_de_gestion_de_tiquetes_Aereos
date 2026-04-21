namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;

public interface IReservationDetailRepository
{
    Task<ReservationDetailAggregate?>             GetByIdAsync(ReservationDetailId id,                      CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDetailAggregate>> GetAllAsync(                                               CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDetailAggregate>> GetByReservationAsync(int reservationId,                   CancellationToken cancellationToken = default);
    Task                                          AddAsync(ReservationDetailAggregate reservationDetail,     CancellationToken cancellationToken = default);
    Task                                          UpdateAsync(ReservationDetailAggregate reservationDetail,  CancellationToken cancellationToken = default);
    Task                                          DeleteAsync(ReservationDetailId id,                        CancellationToken cancellationToken = default);
}
