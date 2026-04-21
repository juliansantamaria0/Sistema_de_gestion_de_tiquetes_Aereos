namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;
public sealed class GetAllRolePermissionsUseCase
{
    private readonly IRolePermissionService _service;
    public GetAllRolePermissionsUseCase(IRolePermissionService service) => _service = service;
    public Task<IReadOnlyList<RolePermissionDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
