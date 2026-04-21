namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;

public sealed class GetAllRolesUseCase
{
    private readonly IRoleService _service;
    public GetAllRolesUseCase(IRoleService service) => _service = service;
    public Task<IReadOnlyList<RoleDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
