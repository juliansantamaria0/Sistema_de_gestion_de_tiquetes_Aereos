namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
public sealed class DeletePermissionUseCase
{
    private readonly IPermissionService _service;
    public DeletePermissionUseCase(IPermissionService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
