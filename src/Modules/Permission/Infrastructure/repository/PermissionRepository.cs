namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;
    public PermissionRepository(AppDbContext context) => _context = context;

    public async Task<PermissionAggregate?> GetByIdAsync(PermissionId id, CancellationToken ct = default)
    {
        var e = await _context.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PermissionId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<PermissionAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task AddAsync(PermissionAggregate entity, CancellationToken ct = default)
        => await _context.Permissions.AddAsync(ToEntity(entity), ct);

    public void Update(PermissionAggregate entity) => _context.Permissions.Update(ToEntity(entity));

    public void Delete(PermissionAggregate entity)
        => _context.Permissions.Remove(new PermissionEntity { PermissionId = entity.Id.Value });

    private static PermissionAggregate ToAggregate(PermissionEntity e) =>
        PermissionAggregate.Reconstitute(e.PermissionId, e.Name, e.Description);

    private static PermissionEntity ToEntity(PermissionAggregate a) =>
        new() { PermissionId = a.Id.Value, Name = a.Name, Description = a.Description };
}
