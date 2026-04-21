namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class SeatStatusRepository : ISeatStatusRepository
{
    private readonly AppDbContext _context;

    public SeatStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static SeatStatusAggregate ToDomain(SeatStatusEntity entity)
        => new(new SeatStatusId(entity.Id), entity.Name);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<SeatStatusAggregate?> GetByIdAsync(
        SeatStatusId      id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.SeatStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<SeatStatusAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.SeatStatuses
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        SeatStatusAggregate seatStatus,
        CancellationToken   cancellationToken = default)
    {
        var entity = new SeatStatusEntity { Name = seatStatus.Name };
        await _context.SeatStatuses.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        SeatStatusAggregate seatStatus,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.SeatStatuses
            .FirstOrDefaultAsync(e => e.Id == seatStatus.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"SeatStatusEntity with id {seatStatus.Id.Value} not found.");

        entity.Name = seatStatus.Name;
        _context.SeatStatuses.Update(entity);
    }

    public async Task DeleteAsync(
        SeatStatusId      id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.SeatStatuses
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"SeatStatusEntity with id {id.Value} not found.");

        _context.SeatStatuses.Remove(entity);
    }
}
