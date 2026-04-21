namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class NationalityRepository : INationalityRepository
{
    private readonly AppDbContext _context;
    public NationalityRepository(AppDbContext context) => _context = context;

    public async Task<NationalityAggregate?> GetByIdAsync(NationalityId id, CancellationToken ct = default)
    {
        var e = await _context.Nationalities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NationalityId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<NationalityAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.Nationalities
            .AsNoTracking()
            .OrderBy(x => x.Demonym)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task AddAsync(NationalityAggregate entity, CancellationToken ct = default)
        => await _context.Nationalities.AddAsync(ToEntity(entity), ct);

    public void Update(NationalityAggregate entity)
        => _context.Nationalities.Update(ToEntity(entity));

    public void Delete(NationalityAggregate entity)
        => _context.Nationalities.Remove(new NationalityEntity { NationalityId = entity.Id.Value });

    private static NationalityAggregate ToAggregate(NationalityEntity e) =>
        NationalityAggregate.Reconstitute(e.NationalityId, e.CountryId, e.Demonym);

    private static NationalityEntity ToEntity(NationalityAggregate a) =>
        new() { NationalityId = a.Id.Value, CountryId = a.CountryId, Demonym = a.Demonym };
}
