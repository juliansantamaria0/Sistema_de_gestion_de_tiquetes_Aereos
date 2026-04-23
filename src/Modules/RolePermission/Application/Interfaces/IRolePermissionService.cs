namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;

public interface IRolePermissionService
{
    Task<RolePermissionDto>               CreateAsync(CreateRolePermissionRequest  request, CancellationToken ct = default);
    Task<RolePermissionDto?>              GetByIdAsync(int id,                              CancellationToken ct = default);
    Task<IReadOnlyList<RolePermissionDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RolePermissionDto>> GetByRoleIdAsync(int roleId, CancellationToken ct = default);
    
    Task                                  DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record RolePermissionDto(int RolePermissionId, int RoleId, int PermissionId);
public sealed record CreateRolePermissionRequest(int RoleId, int PermissionId);

