namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightCrewRepository : IFlightCrewRepository
{
    private readonly AppDbContext _context;

    public FlightCrewRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static FlightCrewAggregate ToDomain(FlightCrewEntity entity)
        => new(
            new FlightCrewId(entity.Id),
            entity.ScheduledFlightId,
            entity.EmployeeId,
            entity.CrewRoleId,
            entity.CreatedAt);

    

    public async Task<FlightCrewAggregate?> GetByIdAsync(
        FlightCrewId      id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightCrews
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<FlightCrewAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightCrews
            .AsNoTracking()
            .OrderBy(e => e.ScheduledFlightId)
            .ThenBy(e => e.EmployeeId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightCrewAggregate>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightCrews
            .AsNoTracking()
            .Where(e => e.ScheduledFlightId == scheduledFlightId)
            .OrderBy(e => e.CrewRoleId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        FlightCrewAggregate flightCrew,
        CancellationToken   cancellationToken = default)
    {
        var entity = new FlightCrewEntity
        {
            ScheduledFlightId = flightCrew.ScheduledFlightId,
            EmployeeId        = flightCrew.EmployeeId,
            CrewRoleId        = flightCrew.CrewRoleId,
            CreatedAt         = flightCrew.CreatedAt
        };
        await _context.FlightCrews.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        FlightCrewAggregate flightCrew,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.FlightCrews
            .FirstOrDefaultAsync(e => e.Id == flightCrew.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightCrewEntity with id {flightCrew.Id.Value} not found.");

        
        
        entity.CrewRoleId = flightCrew.CrewRoleId;

        _context.FlightCrews.Update(entity);
    }

    public async Task DeleteAsync(
        FlightCrewId      id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightCrews
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightCrewEntity with id {id.Value} not found.");

        _context.FlightCrews.Remove(entity);
    }
}
