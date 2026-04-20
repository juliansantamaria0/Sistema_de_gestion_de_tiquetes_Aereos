namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.Interfaces;

public interface IFlightDelayService
{
    Task<FlightDelayDto?>             GetByIdAsync(int id,                                                          CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightDelayDto>> GetAllAsync(                                                                  CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightDelayDto>> GetByFlightAsync(int scheduledFlightId,                                       CancellationToken cancellationToken = default);
    Task<FlightDelayDto>              CreateAsync(int scheduledFlightId, int delayReasonId, int delayMinutes,       CancellationToken cancellationToken = default);
    Task                              AdjustDelayAsync(int id, int delayMinutes,                                    CancellationToken cancellationToken = default);
    Task                              DeleteAsync(int id,                                                           CancellationToken cancellationToken = default);
}

public sealed record FlightDelayDto(
    int      Id,
    int      ScheduledFlightId,
    int      DelayReasonId,
    int      DelayMinutes,
    DateTime ReportedAt);
