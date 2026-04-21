namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.Interfaces;

public interface IFlightCrewService
{
    Task<FlightCrewDto?>             GetByIdAsync(int id,                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCrewDto>> GetAllAsync(                                                      CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightCrewDto>> GetByFlightAsync(int scheduledFlightId,                           CancellationToken cancellationToken = default);
    Task<FlightCrewDto>              CreateAsync(int scheduledFlightId, int employeeId, int crewRoleId, CancellationToken cancellationToken = default);
    Task                             UpdateAsync(int id, int crewRoleId,                               CancellationToken cancellationToken = default);
    Task                             DeleteAsync(int id,                                               CancellationToken cancellationToken = default);
}

public sealed record FlightCrewDto(
    int      Id,
    int      ScheduledFlightId,
    int      EmployeeId,
    int      CrewRoleId,
    DateTime CreatedAt);
