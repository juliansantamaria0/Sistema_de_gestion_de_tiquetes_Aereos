namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;

public sealed class CreateRoleUseCase
{
    private readonly IRoleService _service;
    public CreateRoleUseCase(IRoleService service) => _service = service;
    public Task<RoleDto> ExecuteAsync(CreateRoleRequest request, CancellationToken ct = default)
        => _service.CreateAsync(request, ct);
}
