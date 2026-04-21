namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.Interfaces;

public interface ISeatMapService
{
    Task<SeatMapDto?>             GetByIdAsync(int id,                                                                   CancellationToken cancellationToken = default);
    Task<IEnumerable<SeatMapDto>> GetAllAsync(                                                                           CancellationToken cancellationToken = default);
    Task<IEnumerable<SeatMapDto>> GetByAircraftTypeAsync(int aircraftTypeId,                                             CancellationToken cancellationToken = default);
    Task<SeatMapDto>              CreateAsync(int aircraftTypeId, string seatNumber, int cabinClassId, string? seatFeatures, CancellationToken cancellationToken = default);
    Task                          UpdateAsync(int id, int cabinClassId, string? seatFeatures,                            CancellationToken cancellationToken = default);
    Task                          DeleteAsync(int id,                                                                    CancellationToken cancellationToken = default);
}

public sealed record SeatMapDto(
    int     Id,
    int     AircraftTypeId,
    string  SeatNumber,
    int     CabinClassId,
    string? SeatFeatures);
