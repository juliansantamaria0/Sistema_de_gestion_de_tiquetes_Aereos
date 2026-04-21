namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;

public sealed class GetRoleByIdUseCase
{
    private readonly IRoleService _service;
    public GetRoleByIdUseCase(IRoleService service) => _service = service;
    public Task<RoleDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
