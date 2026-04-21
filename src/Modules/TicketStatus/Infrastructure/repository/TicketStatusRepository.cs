namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class TicketStatusRepository : ITicketStatusRepository
{
    private readonly AppDbContext _context;

    public TicketStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    private static TicketStatusAggregate ToDomain(TicketStatusEntity entity)
        => new(new TicketStatusId(entity.Id), entity.Name);

    public async Task<TicketStatusAggregate?> GetByIdAsync(
        TicketStatusId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.TicketStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<TicketStatusAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.TicketStatuses
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        TicketStatusAggregate ticketStatus,
        CancellationToken     cancellationToken = default)
    {
        var entity = new TicketStatusEntity { Name = ticketStatus.Name };
        await _context.TicketStatuses.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        TicketStatusAggregate ticketStatus,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.TicketStatuses
            .FirstOrDefaultAsync(e => e.Id == ticketStatus.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketStatusEntity with id {ticketStatus.Id.Value} not found.");

        entity.Name = ticketStatus.Name;
        _context.TicketStatuses.Update(entity);
    }

    public async Task DeleteAsync(
        TicketStatusId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.TicketStatuses
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketStatusEntity with id {id.Value} not found.");

        _context.TicketStatuses.Remove(entity);
    }
}
