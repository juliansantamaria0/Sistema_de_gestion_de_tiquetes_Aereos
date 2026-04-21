namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;

/// <summary>Obtiene todos los permisos asignados a un rol específico.</summary>
public sealed class GetRolePermissionsByRoleUseCase
{
    private readonly IRolePermissionService _service;
    public GetRolePermissionsByRoleUseCase(IRolePermissionService service) => _service = service;
    public Task<IReadOnlyList<RolePermissionDto>> ExecuteAsync(int roleId, CancellationToken ct = default)
        => _service.GetByRoleIdAsync(roleId, ct);
}
