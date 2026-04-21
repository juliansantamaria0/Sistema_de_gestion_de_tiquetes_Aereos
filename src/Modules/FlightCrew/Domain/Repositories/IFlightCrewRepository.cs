namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;

public interface IFlightCrewRepository
{
    Task<FlightCrewAggregate?>             GetByIdAsync(FlightCrewId id,                    CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCrewAggregate>> GetAllAsync(                                     CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCrewAggregate>> GetByFlightAsync(int scheduledFlightId,          CancellationToken cancellationToken = default);
    Task                                   AddAsync(FlightCrewAggregate flightCrew,         CancellationToken cancellationToken = default);
    Task                                   UpdateAsync(FlightCrewAggregate flightCrew,      CancellationToken cancellationToken = default);
    Task                                   DeleteAsync(FlightCrewId id,                     CancellationToken cancellationToken = default);
}
