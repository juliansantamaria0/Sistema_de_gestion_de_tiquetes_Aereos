namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.ValueObject;

public interface IFlightStatusHistoryRepository
{
    Task<FlightStatusHistoryAggregate?>             GetByIdAsync(FlightStatusHistoryId id,       CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightStatusHistoryAggregate>> GetAllAsync(                                  CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightStatusHistoryAggregate>> GetByFlightAsync(int scheduledFlightId,      CancellationToken cancellationToken = default);
    Task                                            AddAsync(FlightStatusHistoryAggregate entry, CancellationToken cancellationToken = default);
    Task                                            DeleteAsync(FlightStatusHistoryId id,        CancellationToken cancellationToken = default);
}
