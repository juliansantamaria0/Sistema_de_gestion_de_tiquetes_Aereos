namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.Interfaces;

public interface IRouteScheduleService
{
    Task<RouteScheduleDto?>             GetByIdAsync(int id,                                                   CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteScheduleDto>> GetAllAsync(                                                           CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteScheduleDto>> GetByBaseFlightAsync(int baseFlightId,                                 CancellationToken cancellationToken = default);
    Task<RouteScheduleDto>              CreateAsync(int baseFlightId, byte dayOfWeek, TimeOnly departureTime,  CancellationToken cancellationToken = default);
    Task                                UpdateAsync(int id, byte dayOfWeek, TimeOnly departureTime,            CancellationToken cancellationToken = default);
    Task                                DeleteAsync(int id,                                                    CancellationToken cancellationToken = default);
}

public sealed record RouteScheduleDto(
    int      Id,
    int      BaseFlightId,
    byte     DayOfWeek,
    string   DayOfWeekName,
    TimeOnly DepartureTime);
