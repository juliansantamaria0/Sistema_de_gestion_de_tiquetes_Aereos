namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;

public interface ITicketRepository
{
    Task<TicketAggregate?>             GetByIdAsync(TicketId id,                              CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketAggregate>> GetAllAsync(                                            CancellationToken cancellationToken = default);
    Task<TicketAggregate?>             GetByReservationDetailAsync(int reservationDetailId,    CancellationToken cancellationToken = default);
    Task                               AddAsync(TicketAggregate ticket,                        CancellationToken cancellationToken = default);
    Task                               UpdateAsync(TicketAggregate ticket,                     CancellationToken cancellationToken = default);
    Task                               DeleteAsync(TicketId id,                                CancellationToken cancellationToken = default);

    Task<bool> TicketStatusExistsAsync(int ticketStatusId, CancellationToken cancellationToken = default);

    Task<bool> TicketCodeExistsAsync(string normalizedCode, CancellationToken cancellationToken = default);

    Task<bool> TicketExistsForReservationDetailAsync(int reservationDetailId, CancellationToken cancellationToken = default);

    Task<TicketAggregate> IssueTicketWithHistoryAsync(
        string ticketCodeNormalized,
        int reservationDetailId,
        int ticketStatusId,
        CancellationToken cancellationToken = default);
}
