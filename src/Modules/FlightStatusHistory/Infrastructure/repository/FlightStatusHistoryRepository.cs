namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightStatusHistoryRepository : IFlightStatusHistoryRepository
{
    private readonly AppDbContext _context;

    public FlightStatusHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    private static FlightStatusHistoryAggregate ToDomain(FlightStatusHistoryEntity entity)
        => new(
            new FlightStatusHistoryId(entity.Id),
            entity.ScheduledFlightId,
            entity.FlightStatusId,
            entity.ChangedAt,
            entity.Notes);

    public async Task<FlightStatusHistoryAggregate?> GetByIdAsync(
        FlightStatusHistoryId id,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.FlightStatusHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<FlightStatusHistoryAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightStatusHistories
            .AsNoTracking()
            .OrderByDescending(e => e.ChangedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightStatusHistoryAggregate>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightStatusHistories
            .AsNoTracking()
            .Where(e => e.ScheduledFlightId == scheduledFlightId)
            .OrderBy(e => e.ChangedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        FlightStatusHistoryAggregate entry,
        CancellationToken            cancellationToken = default)
    {
        var entity = new FlightStatusHistoryEntity
        {
            ScheduledFlightId = entry.ScheduledFlightId,
            FlightStatusId    = entry.FlightStatusId,
            ChangedAt         = entry.ChangedAt,
            Notes             = entry.Notes
        };
        await _context.FlightStatusHistories.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(
        FlightStatusHistoryId id,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.FlightStatusHistories
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightStatusHistoryEntity with id {id.Value} not found.");

        _context.FlightStatusHistories.Remove(entity);
    }
}
