namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.Interfaces;

public interface IFlightCancellationService
{
    Task<FlightCancellationDto?>             GetByIdAsync(int id,                                                                    CancellationToken cancellationToken = default);
    Task<FlightCancellationDto?>             GetByFlightAsync(int scheduledFlightId,                                                 CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCancellationDto>> GetAllAsync(                                                                            CancellationToken cancellationToken = default);
    Task<FlightCancellationDto>              CreateAsync(int scheduledFlightId, int cancellationReasonId, string? notes,             CancellationToken cancellationToken = default);
    Task                                     UpdateNotesAsync(int id, string? notes,                                                 CancellationToken cancellationToken = default);
    Task                                     DeleteAsync(int id,                                                                     CancellationToken cancellationToken = default);
}

public sealed record FlightCancellationDto(
    int      Id,
    int      ScheduledFlightId,
    int      CancellationReasonId,
    DateTime CancelledAt,
    string?  Notes);
