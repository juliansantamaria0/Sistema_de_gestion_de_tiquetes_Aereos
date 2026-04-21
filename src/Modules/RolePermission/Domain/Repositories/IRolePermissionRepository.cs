namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.ValueObject;

public interface IRolePermissionRepository
{
    Task<RolePermissionAggregate?>          GetByIdAsync(RolePermissionId id, CancellationToken ct = default);
    Task<IReadOnlyList<RolePermissionAggregate>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RolePermissionAggregate>> GetByRoleIdAsync(int roleId, CancellationToken ct = default);
    Task AddAsync(RolePermissionAggregate entity, CancellationToken ct = default);
    void Delete(RolePermissionAggregate entity);
}
