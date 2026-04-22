namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class ChangeTicketStatusUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTicketStatusUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int id,
        int ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ticket with id {id} was not found.");

        if (!await _context.TicketStatuses.AsNoTracking().AnyAsync(x => x.Id == ticketStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de tiquete con id {ticketStatusId}.");

        if (ticket.TicketStatusId == ticketStatusId)
            throw new InvalidOperationException("El tiquete ya tiene ese estado.");

        var now = DateTime.UtcNow;
        ticket.TicketStatusId = ticketStatusId;
        ticket.UpdatedAt = now;

        await _context.AddTicketStatusHistoryAsync(
            ticket.Id,
            ticketStatusId,
            "Cambio de estado de tiquete",
            now,
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
