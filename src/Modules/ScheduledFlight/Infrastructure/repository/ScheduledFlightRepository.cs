namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class ScheduledFlightRepository : IScheduledFlightRepository
{
    private readonly AppDbContext _context;

    public ScheduledFlightRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static ScheduledFlightAggregate ToDomain(ScheduledFlightEntity entity)
        => ScheduledFlightAggregate.Reconstitute(
            new ScheduledFlightId(entity.Id),
            entity.BaseFlightId,
            entity.AircraftId,
            entity.GateId,
            entity.DepartureDate,
            entity.DepartureTime,
            entity.EstimatedArrivalDatetime,
            entity.FlightStatusId,
            entity.CreatedAt,
            entity.UpdatedAt);

    

    public async Task<ScheduledFlightAggregate?> GetByIdAsync(
        ScheduledFlightId id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.ScheduledFlights
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<ScheduledFlightAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ScheduledFlights
            .AsNoTracking()
            .OrderByDescending(e => e.DepartureDate)
            .ThenBy(e => e.DepartureTime)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<ScheduledFlightAggregate>> GetByBaseFlightAsync(
        int               baseFlightId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ScheduledFlights
            .AsNoTracking()
            .Where(e => e.BaseFlightId == baseFlightId)
            .OrderByDescending(e => e.DepartureDate)
            .ThenBy(e => e.DepartureTime)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<ScheduledFlightAggregate>> GetByDateAsync(
        DateOnly          date,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ScheduledFlights
            .AsNoTracking()
            .Where(e => e.DepartureDate == date)
            .OrderBy(e => e.DepartureTime)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<ScheduledFlightAggregate>> GetByRouteAsync(
        int               routeId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ScheduledFlights
            .AsNoTracking()
            .Join(_context.BaseFlights.AsNoTracking(),
                sf => sf.BaseFlightId,
                bf => bf.Id,
                (sf, bf) => new { sf, bf })
            .Where(x => x.bf.RouteId == routeId)
            .Select(x => x.sf)
            .OrderByDescending(e => e.DepartureDate)
            .ThenBy(e => e.DepartureTime)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        ScheduledFlightAggregate scheduledFlight,
        CancellationToken        cancellationToken = default)
    {
        var entity = new ScheduledFlightEntity
        {
            BaseFlightId             = scheduledFlight.BaseFlightId,
            AircraftId               = scheduledFlight.AircraftId,
            GateId                   = scheduledFlight.GateId,
            DepartureDate            = scheduledFlight.DepartureDate,
            DepartureTime            = scheduledFlight.DepartureTime,
            EstimatedArrivalDatetime = scheduledFlight.EstimatedArrivalDatetime,
            FlightStatusId           = scheduledFlight.FlightStatusId,
            CreatedAt                = scheduledFlight.CreatedAt,
            UpdatedAt                = scheduledFlight.UpdatedAt
        };
        await _context.ScheduledFlights.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        ScheduledFlightAggregate scheduledFlight,
        CancellationToken        cancellationToken = default)
    {
        var entity = await _context.ScheduledFlights
            .FirstOrDefaultAsync(e => e.Id == scheduledFlight.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ScheduledFlightEntity with id {scheduledFlight.Id.Value} not found.");

        entity.AircraftId               = scheduledFlight.AircraftId;
        entity.GateId                   = scheduledFlight.GateId;
        entity.DepartureDate            = scheduledFlight.DepartureDate;
        entity.DepartureTime            = scheduledFlight.DepartureTime;
        entity.EstimatedArrivalDatetime = scheduledFlight.EstimatedArrivalDatetime;
        entity.FlightStatusId           = scheduledFlight.FlightStatusId;
        entity.UpdatedAt                = scheduledFlight.UpdatedAt;

        _context.ScheduledFlights.Update(entity);
    }

    public async Task DeleteAsync(
        ScheduledFlightId id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.ScheduledFlights
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ScheduledFlightEntity with id {id.Value} not found.");

        _context.ScheduledFlights.Remove(entity);
    }
}
