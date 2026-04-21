namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;
    public RoleRepository(AppDbContext context) => _context = context;

    public async Task<RoleAggregate?> GetByIdAsync(RoleId id, CancellationToken ct = default)
    {
        var e = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RoleId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<RoleAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task AddAsync(RoleAggregate entity, CancellationToken ct = default)
        => await _context.Roles.AddAsync(ToEntity(entity), ct);

    public void Update(RoleAggregate entity) => _context.Roles.Update(ToEntity(entity));

    public void Delete(RoleAggregate entity)
        => _context.Roles.Remove(new RoleEntity { RoleId = entity.Id.Value });

    private static RoleAggregate ToAggregate(RoleEntity e) =>
        RoleAggregate.Reconstitute(e.RoleId, e.Name, e.IsActive);

    private static RoleEntity ToEntity(RoleAggregate a) =>
        new() { RoleId = a.Id.Value, Name = a.Name, IsActive = a.IsActive };
}
