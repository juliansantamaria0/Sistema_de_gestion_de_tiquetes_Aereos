namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class SeatMapRepository : ISeatMapRepository
{
    private readonly AppDbContext _context;

    public SeatMapRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static SeatMapAggregate ToDomain(SeatMapEntity entity)
        => new(
            new SeatMapId(entity.Id),
            entity.AircraftTypeId,
            entity.SeatNumber,
            entity.CabinClassId,
            entity.SeatFeatures);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<SeatMapAggregate?> GetByIdAsync(
        SeatMapId         id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.SeatMaps
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<SeatMapAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.SeatMaps
            .AsNoTracking()
            .OrderBy(e => e.AircraftTypeId)
            .ThenBy(e => e.SeatNumber)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<SeatMapAggregate>> GetByAircraftTypeAsync(
        int               aircraftTypeId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.SeatMaps
            .AsNoTracking()
            .Where(e => e.AircraftTypeId == aircraftTypeId)
            .OrderBy(e => e.SeatNumber)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        SeatMapAggregate  seatMap,
        CancellationToken cancellationToken = default)
    {
        var entity = new SeatMapEntity
        {
            AircraftTypeId = seatMap.AircraftTypeId,
            SeatNumber     = seatMap.SeatNumber,
            CabinClassId   = seatMap.CabinClassId,
            SeatFeatures   = seatMap.SeatFeatures
        };
        await _context.SeatMaps.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        SeatMapAggregate  seatMap,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.SeatMaps
            .FirstOrDefaultAsync(e => e.Id == seatMap.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"SeatMapEntity with id {seatMap.Id.Value} not found.");

        // AircraftTypeId y SeatNumber son la clave de negocio — no se modifican.
        entity.CabinClassId = seatMap.CabinClassId;
        entity.SeatFeatures = seatMap.SeatFeatures;

        _context.SeatMaps.Update(entity);
    }

    public async Task DeleteAsync(
        SeatMapId         id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.SeatMaps
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"SeatMapEntity with id {id.Value} not found.");

        _context.SeatMaps.Remove(entity);
    }
}
