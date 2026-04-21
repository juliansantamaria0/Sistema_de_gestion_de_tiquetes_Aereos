namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AppDbContext _context;
    public RolePermissionRepository(AppDbContext context) => _context = context;

    public async Task<RolePermissionAggregate?> GetByIdAsync(RolePermissionId id, CancellationToken ct = default)
    {
        var e = await _context.RolePermissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RolePermissionId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<RolePermissionAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.RolePermissions
            .AsNoTracking()
            .OrderBy(x => x.RoleId).ThenBy(x => x.PermissionId)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task<IReadOnlyList<RolePermissionAggregate>> GetByRoleIdAsync(int roleId, CancellationToken ct = default)
    {
        var list = await _context.RolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .OrderBy(x => x.PermissionId)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task AddAsync(RolePermissionAggregate entity, CancellationToken ct = default)
        => await _context.RolePermissions.AddAsync(ToEntity(entity), ct);

    public void Delete(RolePermissionAggregate entity)
        => _context.RolePermissions.Remove(new RolePermissionEntity { RolePermissionId = entity.Id.Value });

    private static RolePermissionAggregate ToAggregate(RolePermissionEntity e) =>
        RolePermissionAggregate.Reconstitute(e.RolePermissionId, e.RoleId, e.PermissionId);

    private static RolePermissionEntity ToEntity(RolePermissionAggregate a) =>
        new() { RolePermissionId = a.Id.Value, RoleId = a.RoleId, PermissionId = a.PermissionId };
}
