namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static TicketAggregate ToDomain(TicketEntity entity)
        => new(
            new TicketId(entity.Id),
            entity.TicketCode,
            entity.ReservationDetailId,
            entity.IssueDate,
            entity.TicketStatusId,
            entity.CreatedAt,
            entity.UpdatedAt);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<TicketAggregate?> GetByIdAsync(
        TicketId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<TicketAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Tickets
            .AsNoTracking()
            .OrderByDescending(e => e.IssueDate)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<TicketAggregate?> GetByReservationDetailAsync(
        int               reservationDetailId,
        CancellationToken cancellationToken = default)
    {
        // reservation_detail_id es UNIQUE — FirstOrDefault es correcto.
        var entity = await _context.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ReservationDetailId == reservationDetailId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(
        TicketAggregate   ticket,
        CancellationToken cancellationToken = default)
    {
        var entity = new TicketEntity
        {
            TicketCode          = ticket.TicketCode,
            ReservationDetailId = ticket.ReservationDetailId,
            IssueDate           = ticket.IssueDate,
            TicketStatusId      = ticket.TicketStatusId,
            CreatedAt           = ticket.CreatedAt,
            UpdatedAt           = ticket.UpdatedAt
        };
        await _context.Tickets.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        TicketAggregate   ticket,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tickets
            .FirstOrDefaultAsync(e => e.Id == ticket.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketEntity with id {ticket.Id.Value} not found.");

        // Solo TicketStatusId y UpdatedAt son mutables.
        // TicketCode, ReservationDetailId e IssueDate son inmutables.
        entity.TicketStatusId = ticket.TicketStatusId;
        entity.UpdatedAt      = ticket.UpdatedAt;

        _context.Tickets.Update(entity);
    }

    public async Task DeleteAsync(
        TicketId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tickets
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketEntity with id {id.Value} not found.");

        _context.Tickets.Remove(entity);
    }
}
