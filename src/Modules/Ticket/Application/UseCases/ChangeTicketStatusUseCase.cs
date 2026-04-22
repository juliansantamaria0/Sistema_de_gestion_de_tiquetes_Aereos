namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Cambia el estado del tiquete (ISSUED → USED, ISSUED → CANCELLED, etc.)
/// y registra trazabilidad automáticamente.
/// </summary>
public sealed class ChangeTicketStatusUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork  _unitOfWork;

    public ChangeTicketStatusUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ticket with id {id} was not found.");

        if (!await _context.TicketStatuses.AsNoTracking().AnyAsync(x => x.Id == ticketStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de tiquete con id {ticketStatusId}.");

        ticket.TicketStatusId = ticketStatusId;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.TicketStatusHistories.AddAsync(new TicketStatusHistoryEntity
        {
            TicketId = ticket.Id,
            TicketStatusId = ticketStatusId,
            ChangedAt = DateTime.UtcNow,
            Notes = "Cambio de estado de tiquete"
        }, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
