namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class CreateTicketUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketAggregate> ExecuteAsync(
        string ticketCode,
        int reservationDetailId,
        int ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var normalizedCode = ticketCode.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new InvalidOperationException("El código del tiquete es obligatorio.");

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

        if (await _context.Tickets.AsNoTracking().AnyAsync(x => x.TicketCode == normalizedCode, cancellationToken))
            throw new InvalidOperationException($"Ya existe un tiquete con el código {normalizedCode}.");

        if (!await _context.TicketStatuses.AsNoTracking().AnyAsync(x => x.Id == ticketStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de tiquete con id {ticketStatusId}.");

        var ticketEntity = new TicketEntity
        {
            TicketCode = normalizedCode,
            ReservationDetailId = reservationDetailId,
            IssueDate = now,
            TicketStatusId = ticketStatusId,
            CreatedAt = now,
            UpdatedAt = null
        };

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        await _context.Tickets.AddAsync(ticketEntity, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await _context.AddTicketStatusHistoryAsync(
            ticketEntity.Id,
            ticketStatusId,
            "Tiquete emitido",
            now,
            cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

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
