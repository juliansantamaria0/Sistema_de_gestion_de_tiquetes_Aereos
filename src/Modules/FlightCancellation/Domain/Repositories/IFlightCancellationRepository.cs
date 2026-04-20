namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;

public interface IFlightCancellationRepository
{
    Task<FlightCancellationAggregate?>             GetByIdAsync(FlightCancellationId id,                       CancellationToken cancellationToken = default);
    Task<FlightCancellationAggregate?>             GetByFlightAsync(int scheduledFlightId,                     CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCancellationAggregate>> GetAllAsync(                                                 CancellationToken cancellationToken = default);
    Task                                           AddAsync(FlightCancellationAggregate flightCancellation,    CancellationToken cancellationToken = default);
    Task                                           UpdateAsync(FlightCancellationAggregate flightCancellation, CancellationToken cancellationToken = default);
    Task                                           DeleteAsync(FlightCancellationId id,                        CancellationToken cancellationToken = default);
}
