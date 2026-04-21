namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightCabinPriceRepository : IFlightCabinPriceRepository
{
    private readonly AppDbContext _context;

    public FlightCabinPriceRepository(AppDbContext context) => _context = context;

    private static FlightCabinPriceAggregate ToDomain(FlightCabinPriceEntity e)
        => new(new FlightCabinPriceId(e.Id),
               e.ScheduledFlightId, e.CabinClassId, e.FareTypeId, e.Price);

    public async Task<FlightCabinPriceAggregate?> GetByIdAsync(
        FlightCabinPriceId id, CancellationToken ct = default)
    {
        var e = await _context.FlightCabinPrices.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<FlightCabinPriceAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.FlightCabinPrices.AsNoTracking()
            .OrderBy(x => x.ScheduledFlightId)
            .ThenBy(x => x.CabinClassId)
            .ThenBy(x => x.FareTypeId)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightCabinPriceAggregate>> GetByFlightAsync(
        int scheduledFlightId, CancellationToken ct = default)
    {
        var entities = await _context.FlightCabinPrices.AsNoTracking()
            .Where(x => x.ScheduledFlightId == scheduledFlightId)
            .OrderBy(x => x.CabinClassId)
            .ThenBy(x => x.FareTypeId)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(FlightCabinPriceAggregate fcp, CancellationToken ct = default)
    {
        await _context.FlightCabinPrices.AddAsync(new FlightCabinPriceEntity
        {
            ScheduledFlightId = fcp.ScheduledFlightId,
            CabinClassId      = fcp.CabinClassId,
            FareTypeId        = fcp.FareTypeId,
            Price             = fcp.Price
        }, ct);
    }

    public async Task UpdateAsync(FlightCabinPriceAggregate fcp, CancellationToken ct = default)
    {
        var e = await _context.FlightCabinPrices
            .FirstOrDefaultAsync(x => x.Id == fcp.Id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"FlightCabinPriceEntity with id {fcp.Id.Value} not found.");

        // La terna (ScheduledFlightId, CabinClassId, FareTypeId) es inmutable.
        e.Price = fcp.Price;
        _context.FlightCabinPrices.Update(e);
    }

    public async Task DeleteAsync(FlightCabinPriceId id, CancellationToken ct = default)
    {
        var e = await _context.FlightCabinPrices
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"FlightCabinPriceEntity with id {id.Value} not found.");
        _context.FlightCabinPrices.Remove(e);
    }
}
