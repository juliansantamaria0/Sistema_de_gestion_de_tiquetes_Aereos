namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;

public interface IScheduledFlightService
{
    Task<ScheduledFlightDto?>             GetByIdAsync(int id,                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduledFlightDto>> GetAllAsync(                                               CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduledFlightDto>> GetByBaseFlightAsync(int baseFlightId,                     CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduledFlightDto>> GetByDateAsync(DateOnly date,                              CancellationToken cancellationToken = default);
    Task<ScheduledFlightDto>              CreateAsync(CreateScheduledFlightRequest request,          CancellationToken cancellationToken = default);
    Task                                  UpdateAsync(int id, UpdateScheduledFlightRequest request,  CancellationToken cancellationToken = default);
    Task                                  DeleteAsync(int id,                                        CancellationToken cancellationToken = default);
}

public sealed record ScheduledFlightDto(
    int      Id,
    int      BaseFlightId,
    int      AircraftId,
    int?     GateId,
    DateOnly DepartureDate,
    TimeOnly DepartureTime,
    DateTime EstimatedArrivalDatetime,
    int      FlightStatusId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreateScheduledFlightRequest(
    int      BaseFlightId,
    int      AircraftId,
    int?     GateId,
    DateOnly DepartureDate,
    TimeOnly DepartureTime,
    DateTime EstimatedArrivalDatetime,
    int      FlightStatusId);

public sealed record UpdateScheduledFlightRequest(
    int      AircraftId,
    int?     GateId,
    DateOnly DepartureDate,
    TimeOnly DepartureTime,
    DateTime EstimatedArrivalDatetime,
    int      FlightStatusId);
