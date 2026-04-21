namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;

public interface ISeatMapRepository
{
    Task<SeatMapAggregate?>             GetByIdAsync(SeatMapId id,                        CancellationToken cancellationToken = default);
    Task<IEnumerable<SeatMapAggregate>> GetAllAsync(                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<SeatMapAggregate>> GetByAircraftTypeAsync(int aircraftTypeId,         CancellationToken cancellationToken = default);
    Task                                AddAsync(SeatMapAggregate seatMap,                 CancellationToken cancellationToken = default);
    Task                                UpdateAsync(SeatMapAggregate seatMap,              CancellationToken cancellationToken = default);
    Task                                DeleteAsync(SeatMapId id,                          CancellationToken cancellationToken = default);
}
