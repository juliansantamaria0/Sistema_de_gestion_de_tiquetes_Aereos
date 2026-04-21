namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.ValueObject;

public interface ITicketStatusRepository
{
    Task<TicketStatusAggregate?>             GetByIdAsync(TicketStatusId id,                CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketStatusAggregate>> GetAllAsync(                                    CancellationToken cancellationToken = default);
    Task                                     AddAsync(TicketStatusAggregate ticketStatus,   CancellationToken cancellationToken = default);
    Task                                     UpdateAsync(TicketStatusAggregate ticketStatus,CancellationToken cancellationToken = default);
    Task                                     DeleteAsync(TicketStatusId id,                  CancellationToken cancellationToken = default);
}
