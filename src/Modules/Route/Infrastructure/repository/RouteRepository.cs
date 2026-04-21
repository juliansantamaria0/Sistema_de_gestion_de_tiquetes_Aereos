namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class RouteRepository : IRouteRepository
{
    private readonly AppDbContext _context;

    public RouteRepository(AppDbContext context)
    {
        _context = context;
    }

    private static RouteAggregate ToDomain(RouteEntity entity)
        => new(
            new RouteId(entity.Id),
            entity.OriginAirportId,
            entity.DestinationAirportId,
            entity.CreatedAt);

    public async Task<RouteAggregate?> GetByIdAsync(
        RouteId           id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Routes
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<RouteAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Routes
            .AsNoTracking()
            .OrderBy(e => e.OriginAirportId)
            .ThenBy(e => e.DestinationAirportId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<RouteAggregate?> GetByAirportsAsync(
        int               originId,
        int               destinationId,
        CancellationToken cancellationToken = default)
    {
        // UNIQUE (origin, destination) — FirstOrDefault correcto.
        var entity = await _context.Routes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.OriginAirportId == originId &&
                     e.DestinationAirportId == destinationId,
                cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<RouteAggregate>> GetByOriginAsync(
        int               originAirportId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Routes
            .AsNoTracking()
            .Where(e => e.OriginAirportId == originAirportId)
            .OrderBy(e => e.DestinationAirportId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        RouteAggregate    route,
        CancellationToken cancellationToken = default)
    {
        var entity = new RouteEntity
        {
            OriginAirportId      = route.OriginAirportId,
            DestinationAirportId = route.DestinationAirportId,
            CreatedAt            = route.CreatedAt
        };
        await _context.Routes.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(
        RouteId           id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Routes
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"RouteEntity with id {id.Value} not found.");

        _context.Routes.Remove(entity);
    }
}
