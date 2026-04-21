namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;

public interface IFlightCabinPriceRepository
{
    Task<FlightCabinPriceAggregate?>             GetByIdAsync(FlightCabinPriceId id,                                      CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCabinPriceAggregate>> GetAllAsync(                                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCabinPriceAggregate>> GetByFlightAsync(int scheduledFlightId,                                  CancellationToken cancellationToken = default);
    Task                                         AddAsync(FlightCabinPriceAggregate flightCabinPrice,                    CancellationToken cancellationToken = default);
    Task                                         UpdateAsync(FlightCabinPriceAggregate flightCabinPrice,                 CancellationToken cancellationToken = default);
    Task                                         DeleteAsync(FlightCabinPriceId id,                                      CancellationToken cancellationToken = default);
}
