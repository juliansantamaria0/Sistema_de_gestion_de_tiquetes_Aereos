namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightDelayRepository : IFlightDelayRepository
{
    private readonly AppDbContext _context;

    public FlightDelayRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static FlightDelayAggregate ToDomain(FlightDelayEntity entity)
        => new(
            new FlightDelayId(entity.Id),
            entity.ScheduledFlightId,
            entity.DelayReasonId,
            entity.DelayMinutes,
            entity.ReportedAt);

    

    public async Task<FlightDelayAggregate?> GetByIdAsync(
        FlightDelayId     id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightDelays
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<FlightDelayAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightDelays
            .AsNoTracking()
            .OrderByDescending(e => e.ReportedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightDelayAggregate>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightDelays
            .AsNoTracking()
            .Where(e => e.ScheduledFlightId == scheduledFlightId)
            .OrderBy(e => e.ReportedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        FlightDelayAggregate flightDelay,
        CancellationToken    cancellationToken = default)
    {
        var entity = new FlightDelayEntity
        {
            ScheduledFlightId = flightDelay.ScheduledFlightId,
            DelayReasonId     = flightDelay.DelayReasonId,
            DelayMinutes      = flightDelay.DelayMinutes,
            ReportedAt        = flightDelay.ReportedAt
        };
        await _context.FlightDelays.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        FlightDelayAggregate flightDelay,
        CancellationToken    cancellationToken = default)
    {
        var entity = await _context.FlightDelays
            .FirstOrDefaultAsync(e => e.Id == flightDelay.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightDelayEntity with id {flightDelay.Id.Value} not found.");

        
        
        entity.DelayMinutes = flightDelay.DelayMinutes;

        _context.FlightDelays.Update(entity);
    }

    public async Task DeleteAsync(
        FlightDelayId     id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightDelays
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightDelayEntity with id {id.Value} not found.");

        _context.FlightDelays.Remove(entity);
    }
}
