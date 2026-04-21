namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.Interfaces;

public interface ITicketStatusService
{
    Task<TicketStatusDto?>             GetByIdAsync(int id,            CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketStatusDto>> GetAllAsync(                    CancellationToken cancellationToken = default);
    Task<TicketStatusDto>              CreateAsync(string name,        CancellationToken cancellationToken = default);
    Task                               UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                               DeleteAsync(int id,             CancellationToken cancellationToken = default);
}

public sealed record TicketStatusDto(int Id, string Name);
