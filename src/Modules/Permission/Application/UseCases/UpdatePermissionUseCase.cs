namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
public sealed class UpdatePermissionUseCase
{
    private readonly IPermissionService _service;
    public UpdatePermissionUseCase(IPermissionService service) => _service = service;
    public Task<PermissionDto> ExecuteAsync(int id, UpdatePermissionRequest request, CancellationToken ct = default)
        => _service.UpdateAsync(id, request, ct);
}
