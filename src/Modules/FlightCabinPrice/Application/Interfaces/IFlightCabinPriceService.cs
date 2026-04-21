namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.Interfaces;

public interface IFlightCabinPriceService
{
    Task<FlightCabinPriceDto?>             GetByIdAsync(int id,                                                                    CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCabinPriceDto>> GetAllAsync(                                                                            CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCabinPriceDto>> GetByFlightAsync(int scheduledFlightId,                                                CancellationToken cancellationToken = default);
    Task<FlightCabinPriceDto>              CreateAsync(int scheduledFlightId, int cabinClassId, int fareTypeId, decimal price,    CancellationToken cancellationToken = default);
    Task                                   UpdatePriceAsync(int id, decimal newPrice,                                             CancellationToken cancellationToken = default);
    Task                                   DeleteAsync(int id,                                                                    CancellationToken cancellationToken = default);
}

public sealed record FlightCabinPriceDto(
    int     Id,
    int     ScheduledFlightId,
    int     CabinClassId,
    int     FareTypeId,
    decimal Price);
