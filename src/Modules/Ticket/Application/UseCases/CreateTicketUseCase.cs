namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateTicketUseCase
{
    private readonly ITicketRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public CreateTicketUseCase(ITicketRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketAggregate> ExecuteAsync(
        string            ticketCode,
        int               reservationDetailId,
        int               ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // TicketId(1) es placeholder; EF Core asigna el Id real al insertar.
        var ticket = new TicketAggregate(
            new TicketId(1),
            ticketCode,
            reservationDetailId,
            now,
            ticketStatusId,
            now);

        await _repository.AddAsync(ticket, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return ticket;
    }
}
