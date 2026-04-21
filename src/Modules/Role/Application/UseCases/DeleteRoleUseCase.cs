namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;

public sealed class DeleteRoleUseCase
{
    private readonly IRoleService _service;
    public DeleteRoleUseCase(IRoleService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
