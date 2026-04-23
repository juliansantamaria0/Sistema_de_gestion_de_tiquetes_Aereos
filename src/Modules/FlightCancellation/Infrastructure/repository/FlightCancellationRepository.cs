namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightCancellationRepository : IFlightCancellationRepository
{
    private readonly AppDbContext _context;

    public FlightCancellationRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static FlightCancellationAggregate ToDomain(FlightCancellationEntity entity)
        => new(
            new FlightCancellationId(entity.Id),
            entity.ScheduledFlightId,
            entity.CancellationReasonId,
            entity.CancelledAt,
            entity.Notes);

    

    public async Task<FlightCancellationAggregate?> GetByIdAsync(
        FlightCancellationId id,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.FlightCancellations
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<FlightCancellationAggregate?> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        
        var entity = await _context.FlightCancellations
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ScheduledFlightId == scheduledFlightId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<FlightCancellationAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightCancellations
            .AsNoTracking()
            .OrderByDescending(e => e.CancelledAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        FlightCancellationAggregate flightCancellation,
        CancellationToken           cancellationToken = default)
    {
        var entity = new FlightCancellationEntity
        {
            ScheduledFlightId    = flightCancellation.ScheduledFlightId,
            CancellationReasonId = flightCancellation.CancellationReasonId,
            CancelledAt          = flightCancellation.CancelledAt,
            Notes                = flightCancellation.Notes
        };
        await _context.FlightCancellations.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        FlightCancellationAggregate flightCancellation,
        CancellationToken           cancellationToken = default)
    {
        var entity = await _context.FlightCancellations
            .FirstOrDefaultAsync(e => e.Id == flightCancellation.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightCancellationEntity with id {flightCancellation.Id.Value} not found.");

        
        
        entity.Notes = flightCancellation.Notes;

        _context.FlightCancellations.Update(entity);
    }

    public async Task DeleteAsync(
        FlightCancellationId id,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.FlightCancellations
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightCancellationEntity with id {id.Value} not found.");

        _context.FlightCancellations.Remove(entity);
    }
}
