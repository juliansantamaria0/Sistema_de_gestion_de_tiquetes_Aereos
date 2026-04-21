namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class GenderRepository : IGenderRepository
{
    private readonly AppDbContext _context;

    public GenderRepository(AppDbContext context)
    {
        _context = context;
    }

    private static GenderAggregate ToDomain(GenderEntity entity)
        => new(new GenderId(entity.Id), entity.Name);

    public async Task<GenderAggregate?> GetByIdAsync(
        GenderId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Genders
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<GenderAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Genders
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        GenderAggregate   gender,
        CancellationToken cancellationToken = default)
    {
        var entity = new GenderEntity { Name = gender.Name };
        await _context.Genders.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        GenderAggregate   gender,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Genders
            .FirstOrDefaultAsync(e => e.Id == gender.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"GenderEntity with id {gender.Id.Value} not found.");

        entity.Name = gender.Name;
        _context.Genders.Update(entity);
    }

    public async Task DeleteAsync(
        GenderId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Genders
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"GenderEntity with id {id.Value} not found.");

        _context.Genders.Remove(entity);
    }
}
