namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.Interfaces;

public interface IFlightStatusService
{
    Task<FlightStatusDto?>             GetByIdAsync(int id,            CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightStatusDto>> GetAllAsync(                    CancellationToken cancellationToken = default);
    Task<FlightStatusDto>              CreateAsync(string name,        CancellationToken cancellationToken = default);
    Task                               UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                               DeleteAsync(int id,             CancellationToken cancellationToken = default);
}

public sealed record FlightStatusDto(int Id, string Name);
