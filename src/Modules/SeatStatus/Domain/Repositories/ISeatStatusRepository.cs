namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;

public interface ISeatStatusRepository
{
    Task<SeatStatusAggregate?>             GetByIdAsync(SeatStatusId id,              CancellationToken cancellationToken = default);
    Task<IEnumerable<SeatStatusAggregate>> GetAllAsync(                               CancellationToken cancellationToken = default);
    Task                                   AddAsync(SeatStatusAggregate seatStatus,   CancellationToken cancellationToken = default);
    Task                                   UpdateAsync(SeatStatusAggregate seatStatus,CancellationToken cancellationToken = default);
    Task                                   DeleteAsync(SeatStatusId id,               CancellationToken cancellationToken = default);
}
