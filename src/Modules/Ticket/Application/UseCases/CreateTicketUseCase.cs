namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateTicketUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork  _unitOfWork;

    public CreateTicketUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketAggregate> ExecuteAsync(
        string            ticketCode,
        int               reservationDetailId,
        int               ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var detail = await _context.ReservationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == reservationDetailId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe el detalle de reserva con id {reservationDetailId}.");

        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == detail.ReservationId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe la reserva asociada al detalle {reservationDetailId}.");

        if (!reservation.ConfirmedAt.HasValue || reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("Solo se pueden emitir tiquetes desde reservas válidas y confirmadas.");

        if (await _context.Tickets.AsNoTracking().AnyAsync(x => x.ReservationDetailId == reservationDetailId, cancellationToken))
            throw new InvalidOperationException("Ya existe un tiquete emitido para este detalle de reserva.");

        if (!await _context.TicketStatuses.AsNoTracking().AnyAsync(x => x.Id == ticketStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de tiquete con id {ticketStatusId}.");

        var ticketEntity = new TicketEntity
        {
            TicketCode = ticketCode.Trim().ToUpperInvariant(),
            ReservationDetailId = reservationDetailId,
            IssueDate = now,
            TicketStatusId = ticketStatusId,
            CreatedAt = now,
            UpdatedAt = null
        };

        await _context.Tickets.AddAsync(ticketEntity, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await _context.TicketStatusHistories.AddAsync(new TicketStatusHistoryEntity
        {
            TicketId = ticketEntity.Id,
            TicketStatusId = ticketStatusId,
            ChangedAt = now,
            Notes = "Tiquete emitido"
        }, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new TicketAggregate(
            new TicketId(ticketEntity.Id),
            ticketEntity.TicketCode,
            ticketEntity.ReservationDetailId,
            ticketEntity.IssueDate,
            ticketEntity.TicketStatusId,
            ticketEntity.CreatedAt,
            ticketEntity.UpdatedAt);
    }
}
