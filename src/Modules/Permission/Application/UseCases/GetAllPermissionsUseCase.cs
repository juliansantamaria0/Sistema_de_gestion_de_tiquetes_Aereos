namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
public sealed class GetAllPermissionsUseCase
{
    private readonly IPermissionService _service;
    public GetAllPermissionsUseCase(IPermissionService service) => _service = service;
    public Task<IReadOnlyList<PermissionDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
