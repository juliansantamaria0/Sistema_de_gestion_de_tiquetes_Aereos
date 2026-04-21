namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightStatusRepository : IFlightStatusRepository
{
    private readonly AppDbContext _context;

    public FlightStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    private static FlightStatusAggregate ToDomain(FlightStatusEntity entity)
        => new(new FlightStatusId(entity.Id), entity.Name);

    public async Task<FlightStatusAggregate?> GetByIdAsync(
        FlightStatusId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<FlightStatusAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightStatuses
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        FlightStatusAggregate flightStatus,
        CancellationToken     cancellationToken = default)
    {
        var entity = new FlightStatusEntity { Name = flightStatus.Name };
        await _context.FlightStatuses.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        FlightStatusAggregate flightStatus,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.FlightStatuses
            .FirstOrDefaultAsync(e => e.Id == flightStatus.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightStatusEntity with id {flightStatus.Id.Value} not found.");

        entity.Name = flightStatus.Name;
        _context.FlightStatuses.Update(entity);
    }

    public async Task DeleteAsync(
        FlightStatusId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightStatuses
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightStatusEntity with id {id.Value} not found.");

        _context.FlightStatuses.Remove(entity);
    }
}
