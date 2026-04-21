namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.ValueObject;

public interface IFlightStatusRepository
{
    Task<FlightStatusAggregate?>             GetByIdAsync(FlightStatusId id,                 CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightStatusAggregate>> GetAllAsync(                                     CancellationToken cancellationToken = default);
    Task                                     AddAsync(FlightStatusAggregate flightStatus,    CancellationToken cancellationToken = default);
    Task                                     UpdateAsync(FlightStatusAggregate flightStatus, CancellationToken cancellationToken = default);
    Task                                     DeleteAsync(FlightStatusId id,                  CancellationToken cancellationToken = default);
}
