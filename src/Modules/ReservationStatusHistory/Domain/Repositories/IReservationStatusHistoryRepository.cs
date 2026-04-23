namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.ValueObject;

public interface IReservationStatusHistoryRepository
{
    Task<ReservationStatusHistoryAggregate?>             GetByIdAsync(ReservationStatusHistoryId id,           CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationStatusHistoryAggregate>> GetAllAsync(                                           CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationStatusHistoryAggregate>> GetByReservationAsync(int reservationId,              CancellationToken cancellationToken = default);
    Task                                                 AddAsync(ReservationStatusHistoryAggregate entry,    CancellationToken cancellationToken = default);
    Task                                                 DeleteAsync(ReservationStatusHistoryId id,            CancellationToken cancellationToken = default);
    
}
