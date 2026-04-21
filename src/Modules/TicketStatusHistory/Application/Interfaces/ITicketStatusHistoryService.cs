namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.Interfaces;

public interface ITicketStatusHistoryService
{
    Task<TicketStatusHistoryDto?>             GetByIdAsync(int id,                                                   CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketStatusHistoryDto>> GetAllAsync(                                                           CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketStatusHistoryDto>> GetByTicketAsync(int ticketId,                                        CancellationToken cancellationToken = default);
    Task<TicketStatusHistoryDto>              RecordAsync(int ticketId, int ticketStatusId, string? notes,          CancellationToken cancellationToken = default);
    Task                                      DeleteAsync(int id,                                                   CancellationToken cancellationToken = default);
}

public sealed record TicketStatusHistoryDto(
    int      Id,
    int      TicketId,
    int      TicketStatusId,
    DateTime ChangedAt,
    string?  Notes);
