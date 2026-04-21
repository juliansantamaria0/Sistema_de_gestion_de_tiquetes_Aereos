namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightPromotionRepository : IFlightPromotionRepository
{
    private readonly AppDbContext _context;

    public FlightPromotionRepository(AppDbContext context) => _context = context;

    private static FlightPromotionAggregate ToDomain(FlightPromotionEntity e)
        => new(new FlightPromotionId(e.Id), e.ScheduledFlightId, e.PromotionId);

    public async Task<FlightPromotionAggregate?> GetByIdAsync(
        FlightPromotionId id, CancellationToken ct = default)
    {
        var e = await _context.FlightPromotions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<FlightPromotionAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.FlightPromotions.AsNoTracking()
            .OrderBy(x => x.ScheduledFlightId).ThenBy(x => x.PromotionId)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightPromotionAggregate>> GetByFlightAsync(
        int scheduledFlightId, CancellationToken ct = default)
    {
        var entities = await _context.FlightPromotions.AsNoTracking()
            .Where(x => x.ScheduledFlightId == scheduledFlightId)
            .OrderBy(x => x.PromotionId)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightPromotionAggregate>> GetByPromotionAsync(
        int promotionId, CancellationToken ct = default)
    {
        var entities = await _context.FlightPromotions.AsNoTracking()
            .Where(x => x.PromotionId == promotionId)
            .OrderBy(x => x.ScheduledFlightId)
            .ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(FlightPromotionAggregate fp, CancellationToken ct = default)
    {
        await _context.FlightPromotions.AddAsync(new FlightPromotionEntity
        {
            ScheduledFlightId = fp.ScheduledFlightId,
            PromotionId       = fp.PromotionId
        }, ct);
    }

    public async Task DeleteAsync(FlightPromotionId id, CancellationToken ct = default)
    {
        var e = await _context.FlightPromotions
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"FlightPromotionEntity with id {id.Value} not found.");
        _context.FlightPromotions.Remove(e);
    }
}
