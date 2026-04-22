namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class RecordTicketStatusHistoryUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public RecordTicketStatusHistoryUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketStatusHistoryAggregate> ExecuteAsync(
        int ticketId,
        int ticketStatusId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.Tickets.AsNoTracking().AnyAsync(x => x.Id == ticketId, cancellationToken))
            throw new InvalidOperationException($"No existe el tiquete con id {ticketId}.");

        if (!await _context.TicketStatuses.AsNoTracking().AnyAsync(x => x.Id == ticketStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de tiquete con id {ticketStatusId}.");

        var changedAt = DateTime.UtcNow;
        await _context.AddTicketStatusHistoryAsync(
            ticketId,
            ticketStatusId,
            notes,
            changedAt,
            cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var entity = await _context.TicketStatusHistories
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .FirstAsync(x => x.TicketId == ticketId && x.TicketStatusId == ticketStatusId && x.ChangedAt == changedAt, cancellationToken);

        return new TicketStatusHistoryAggregate(
            new TicketStatusHistoryId(entity.Id),
            entity.TicketId,
            entity.TicketStatusId,
            entity.ChangedAt,
            entity.Notes);
    }
}
