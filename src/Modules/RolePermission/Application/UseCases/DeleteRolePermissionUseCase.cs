namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;
public sealed class DeleteRolePermissionUseCase
{
    private readonly IRolePermissionService _service;
    public DeleteRolePermissionUseCase(IRolePermissionService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
