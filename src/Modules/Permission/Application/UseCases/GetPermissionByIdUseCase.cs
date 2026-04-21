namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
public sealed class GetPermissionByIdUseCase
{
    private readonly IPermissionService _service;
    public GetPermissionByIdUseCase(IPermissionService service) => _service = service;
    public Task<PermissionDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
