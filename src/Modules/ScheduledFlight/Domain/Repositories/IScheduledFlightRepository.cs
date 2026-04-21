namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;

public interface IScheduledFlightRepository
{
    Task<ScheduledFlightAggregate?>             GetByIdAsync(ScheduledFlightId id,                       CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduledFlightAggregate>> GetAllAsync(                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduledFlightAggregate>> GetByBaseFlightAsync(int baseFlightId,                    CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduledFlightAggregate>> GetByDateAsync(DateOnly date,                             CancellationToken cancellationToken = default);
    Task                                        AddAsync(ScheduledFlightAggregate scheduledFlight,        CancellationToken cancellationToken = default);
    Task                                        UpdateAsync(ScheduledFlightAggregate scheduledFlight,     CancellationToken cancellationToken = default);
    Task                                        DeleteAsync(ScheduledFlightId id,                        CancellationToken cancellationToken = default);
}
