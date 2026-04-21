namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Cambia el estado del tiquete (ISSUED → USED, ISSUED → CANCELLED, etc.).
/// ticket_code, reservation_detail_id e issue_date son inmutables.
/// </summary>
public sealed class ChangeTicketStatusUseCase
{
    private readonly ITicketRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public ChangeTicketStatusUseCase(ITicketRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(new TicketId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Ticket with id {id} was not found.");

        ticket.ChangeStatus(ticketStatusId);
        await _repository.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
