namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;

public interface IFlightDelayRepository
{
    Task<FlightDelayAggregate?>             GetByIdAsync(FlightDelayId id,                    CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightDelayAggregate>> GetAllAsync(                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightDelayAggregate>> GetByFlightAsync(int scheduledFlightId,            CancellationToken cancellationToken = default);
    Task                                    AddAsync(FlightDelayAggregate flightDelay,         CancellationToken cancellationToken = default);
    Task                                    UpdateAsync(FlightDelayAggregate flightDelay,      CancellationToken cancellationToken = default);
    Task                                    DeleteAsync(FlightDelayId id,                      CancellationToken cancellationToken = default);
}
