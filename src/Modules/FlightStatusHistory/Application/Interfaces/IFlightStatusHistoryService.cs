namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.Interfaces;

public interface IFlightStatusHistoryService
{
    Task<FlightStatusHistoryDto?>             GetByIdAsync(int id,                                                        CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightStatusHistoryDto>> GetAllAsync(                                                                CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightStatusHistoryDto>> GetByFlightAsync(int scheduledFlightId,                                    CancellationToken cancellationToken = default);
    Task<FlightStatusHistoryDto>              RecordAsync(int scheduledFlightId, int flightStatusId, string? notes,     CancellationToken cancellationToken = default);
    Task                                      DeleteAsync(int id,                                                       CancellationToken cancellationToken = default);
}

public sealed record FlightStatusHistoryDto(
    int      Id,
    int      ScheduledFlightId,
    int      FlightStatusId,
    DateTime ChangedAt,
    string?  Notes);
