namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class RouteScheduleRepository : IRouteScheduleRepository
{
    private readonly AppDbContext _context;

    public RouteScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static RouteScheduleAggregate ToDomain(RouteScheduleEntity entity)
        => new(
            new RouteScheduleId(entity.Id),
            entity.BaseFlightId,
            entity.DayOfWeek,
            entity.DepartureTime);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<RouteScheduleAggregate?> GetByIdAsync(
        RouteScheduleId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.RouteSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<RouteScheduleAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.RouteSchedules
            .AsNoTracking()
            .OrderBy(e => e.BaseFlightId)
            .ThenBy(e => e.DayOfWeek)
            .ThenBy(e => e.DepartureTime)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<RouteScheduleAggregate>> GetByBaseFlightAsync(
        int               baseFlightId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.RouteSchedules
            .AsNoTracking()
            .Where(e => e.BaseFlightId == baseFlightId)
            .OrderBy(e => e.DayOfWeek)
            .ThenBy(e => e.DepartureTime)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        RouteScheduleAggregate routeSchedule,
        CancellationToken      cancellationToken = default)
    {
        var entity = new RouteScheduleEntity
        {
            BaseFlightId  = routeSchedule.BaseFlightId,
            DayOfWeek     = routeSchedule.DayOfWeek,
            DepartureTime = routeSchedule.DepartureTime
        };
        await _context.RouteSchedules.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        RouteScheduleAggregate routeSchedule,
        CancellationToken      cancellationToken = default)
    {
        var entity = await _context.RouteSchedules
            .FirstOrDefaultAsync(e => e.Id == routeSchedule.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"RouteScheduleEntity with id {routeSchedule.Id.Value} not found.");

        entity.DayOfWeek     = routeSchedule.DayOfWeek;
        entity.DepartureTime = routeSchedule.DepartureTime;

        _context.RouteSchedules.Update(entity);
    }

    public async Task DeleteAsync(
        RouteScheduleId   id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.RouteSchedules
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"RouteScheduleEntity with id {id.Value} not found.");

        _context.RouteSchedules.Remove(entity);
    }
}
