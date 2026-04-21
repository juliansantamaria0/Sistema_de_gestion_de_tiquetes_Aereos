namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;

public sealed class UpdateRoleUseCase
{
    private readonly IRoleService _service;
    public UpdateRoleUseCase(IRoleService service) => _service = service;
    public Task<RoleDto> ExecuteAsync(int id, UpdateRoleRequest request, CancellationToken ct = default)
        => _service.UpdateAsync(id, request, ct);
}
