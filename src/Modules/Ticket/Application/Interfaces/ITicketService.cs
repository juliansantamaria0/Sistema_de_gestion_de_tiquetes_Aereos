namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.Interfaces;

public interface ITicketService
{
    Task<TicketDto?>             GetByIdAsync(int id,                                                         CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketDto>> GetAllAsync(                                                                 CancellationToken cancellationToken = default);
    Task<TicketDto?>             GetByReservationDetailAsync(int reservationDetailId,                         CancellationToken cancellationToken = default);
    Task<TicketDto>              CreateAsync(string ticketCode, int reservationDetailId, int ticketStatusId,  CancellationToken cancellationToken = default);
    Task                         ChangeStatusAsync(int id, int ticketStatusId,                               CancellationToken cancellationToken = default);
    Task                         DeleteAsync(int id,                                                         CancellationToken cancellationToken = default);
}

public sealed record TicketDto(
    int      Id,
    string   TicketCode,
    int      ReservationDetailId,
    DateTime IssueDate,
    int      TicketStatusId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
