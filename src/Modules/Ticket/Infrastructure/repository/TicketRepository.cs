namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public TicketRepository(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    private static TicketAggregate ToDomain(TicketEntity entity)
        => new(
            new TicketId(entity.Id),
            entity.TicketCode,
            entity.ReservationDetailId,
            entity.IssueDate,
            entity.TicketStatusId,
            entity.CreatedAt,
            entity.UpdatedAt);

    public async Task<TicketAggregate?> GetByIdAsync(
        TicketId id,
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
        int reservationDetailId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ReservationDetailId == reservationDetailId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<bool> TicketStatusExistsAsync(int ticketStatusId, CancellationToken cancellationToken) =>
        await _context.TicketStatuses.AsNoTracking().AnyAsync(x => x.Id == ticketStatusId, cancellationToken);

    public async Task<bool> TicketCodeExistsAsync(string normalizedCode, CancellationToken cancellationToken) =>
        await _context.Tickets.AsNoTracking().AnyAsync(x => x.TicketCode == normalizedCode, cancellationToken);

    public async Task<bool> TicketExistsForReservationDetailAsync(int reservationDetailId, CancellationToken cancellationToken) =>
        await _context.Tickets.AsNoTracking().AnyAsync(x => x.ReservationDetailId == reservationDetailId, cancellationToken);

    public async Task AddAsync(
        TicketAggregate ticket,
        CancellationToken cancellationToken = default)
    {
        var entity = new TicketEntity
        {
            TicketCode = ticket.TicketCode,
            ReservationDetailId = ticket.ReservationDetailId,
            IssueDate = ticket.IssueDate,
            TicketStatusId = ticket.TicketStatusId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
        await _context.Tickets.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        TicketAggregate ticket,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tickets
            .FirstOrDefaultAsync(e => e.Id == ticket.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketEntity with id {ticket.Id.Value} not found.");

        entity.TicketStatusId = ticket.TicketStatusId;
        entity.UpdatedAt = ticket.UpdatedAt;

        _context.Tickets.Update(entity);
    }

    public async Task DeleteAsync(
        TicketId id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Tickets
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketEntity with id {id.Value} not found.");

        _context.Tickets.Remove(entity);
    }

    public async Task<TicketAggregate> IssueTicketWithHistoryAsync(
        string ticketCodeNormalized,
        int reservationDetailId,
        int ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var ticketEntity = new TicketEntity
        {
            TicketCode = ticketCodeNormalized,
            ReservationDetailId = reservationDetailId,
            IssueDate = now,
            TicketStatusId = ticketStatusId,
            CreatedAt = now,
            UpdatedAt = null
        };

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
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
        });

        return ToDomain(ticketEntity);
    }
}
