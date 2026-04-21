namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class TicketBaggageRepository : ITicketBaggageRepository
{
    private readonly AppDbContext _context;

    public TicketBaggageRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static TicketBaggageAggregate ToDomain(TicketBaggageEntity entity)
        => new(
            new TicketBaggageId(entity.Id),
            entity.TicketId,
            entity.BaggageTypeId,
            entity.Quantity,
            entity.FeeCharged);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<TicketBaggageAggregate?> GetByIdAsync(
        TicketBaggageId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.TicketBaggages
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<TicketBaggageAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.TicketBaggages
            .AsNoTracking()
            .OrderBy(e => e.TicketId)
            .ThenBy(e => e.BaggageTypeId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<TicketBaggageAggregate>> GetByTicketAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.TicketBaggages
            .AsNoTracking()
            .Where(e => e.TicketId == ticketId)
            .OrderBy(e => e.BaggageTypeId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        TicketBaggageAggregate ticketBaggage,
        CancellationToken      cancellationToken = default)
    {
        var entity = new TicketBaggageEntity
        {
            TicketId      = ticketBaggage.TicketId,
            BaggageTypeId = ticketBaggage.BaggageTypeId,
            Quantity      = ticketBaggage.Quantity,
            FeeCharged    = ticketBaggage.FeeCharged
        };
        await _context.TicketBaggages.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        TicketBaggageAggregate ticketBaggage,
        CancellationToken      cancellationToken = default)
    {
        var entity = await _context.TicketBaggages
            .FirstOrDefaultAsync(e => e.Id == ticketBaggage.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketBaggageEntity with id {ticketBaggage.Id.Value} not found.");

        // Solo Quantity y FeeCharged son mutables.
        // TicketId y BaggageTypeId son la clave de negocio — inmutables.
        entity.Quantity   = ticketBaggage.Quantity;
        entity.FeeCharged = ticketBaggage.FeeCharged;

        _context.TicketBaggages.Update(entity);
    }

    public async Task DeleteAsync(
        TicketBaggageId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.TicketBaggages
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"TicketBaggageEntity with id {id.Value} not found.");

        _context.TicketBaggages.Remove(entity);
    }
}
