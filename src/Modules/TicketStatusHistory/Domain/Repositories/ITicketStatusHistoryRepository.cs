namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;

public interface ITicketStatusHistoryRepository
{
    Task<TicketStatusHistoryAggregate?>             GetByIdAsync(TicketStatusHistoryId id,      CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketStatusHistoryAggregate>> GetAllAsync(                                 CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketStatusHistoryAggregate>> GetByTicketAsync(int ticketId,              CancellationToken cancellationToken = default);
    Task                                            AddAsync(TicketStatusHistoryAggregate entry,CancellationToken cancellationToken = default);
    Task                                            DeleteAsync(TicketStatusHistoryId id,       CancellationToken cancellationToken = default);
}
