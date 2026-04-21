namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class CabinClassRepository : ICabinClassRepository
{
    private readonly AppDbContext _context;

    public CabinClassRepository(AppDbContext context) => _context = context;

    private static CabinClassAggregate ToDomain(CabinClassEntity e)
        => new(new CabinClassId(e.Id), e.Name);

    public async Task<CabinClassAggregate?> GetByIdAsync(
        CabinClassId id, CancellationToken ct = default)
    {
        var e = await _context.CabinClasses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<CabinClassAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.CabinClasses.AsNoTracking()
            .OrderBy(x => x.Name).ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(CabinClassAggregate cabinClass, CancellationToken ct = default)
    {
        await _context.CabinClasses.AddAsync(
            new CabinClassEntity { Name = cabinClass.Name }, ct);
    }

    public async Task UpdateAsync(CabinClassAggregate cabinClass, CancellationToken ct = default)
    {
        var e = await _context.CabinClasses
            .FirstOrDefaultAsync(x => x.Id == cabinClass.Id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"CabinClassEntity with id {cabinClass.Id.Value} not found.");
        e.Name = cabinClass.Name;
        _context.CabinClasses.Update(e);
    }

    public async Task DeleteAsync(CabinClassId id, CancellationToken ct = default)
    {
        var e = await _context.CabinClasses
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"CabinClassEntity with id {id.Value} not found.");
        _context.CabinClasses.Remove(e);
    }
}
