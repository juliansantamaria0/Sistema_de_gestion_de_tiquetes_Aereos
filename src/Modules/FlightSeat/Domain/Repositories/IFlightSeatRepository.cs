namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;

public interface IFlightSeatRepository
{
    Task<FlightSeatAggregate?>             GetByIdAsync(FlightSeatId id,                        CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightSeatAggregate>> GetAllAsync(                                          CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightSeatAggregate>> GetByFlightAsync(int scheduledFlightId,               CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightSeatAggregate>> GetAvailableByFlightAsync(int scheduledFlightId,      CancellationToken cancellationToken = default);
    Task                                   AddAsync(FlightSeatAggregate flightSeat,              CancellationToken cancellationToken = default);
    Task                                   UpdateAsync(FlightSeatAggregate flightSeat,           CancellationToken cancellationToken = default);
    Task                                   DeleteAsync(FlightSeatId id,                          CancellationToken cancellationToken = default);
}
