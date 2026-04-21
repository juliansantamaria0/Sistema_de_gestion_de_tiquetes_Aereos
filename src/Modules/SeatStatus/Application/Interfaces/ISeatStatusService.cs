namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.Interfaces;

public interface ISeatStatusService
{
    Task<SeatStatusDto?>             GetByIdAsync(int id,          CancellationToken cancellationToken = default);
    Task<IEnumerable<SeatStatusDto>> GetAllAsync(                  CancellationToken cancellationToken = default);
    Task<SeatStatusDto>              CreateAsync(string name,      CancellationToken cancellationToken = default);
    Task                             UpdateAsync(int id, string name, CancellationToken cancellationToken = default);
    Task                             DeleteAsync(int id,           CancellationToken cancellationToken = default);
}

public sealed record SeatStatusDto(int Id, string Name);
