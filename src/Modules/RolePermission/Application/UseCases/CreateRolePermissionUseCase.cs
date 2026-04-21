namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;
public sealed class CreateRolePermissionUseCase
{
    private readonly IRolePermissionService _service;
    public CreateRolePermissionUseCase(IRolePermissionService service) => _service = service;
    public Task<RolePermissionDto> ExecuteAsync(CreateRolePermissionRequest request, CancellationToken ct = default)
        => _service.CreateAsync(request, ct);
}
