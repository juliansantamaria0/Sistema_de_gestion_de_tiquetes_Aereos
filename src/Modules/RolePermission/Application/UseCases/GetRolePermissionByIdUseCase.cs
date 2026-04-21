namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;
public sealed class GetRolePermissionByIdUseCase
{
    private readonly IRolePermissionService _service;
    public GetRolePermissionByIdUseCase(IRolePermissionService service) => _service = service;
    public Task<RolePermissionDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
