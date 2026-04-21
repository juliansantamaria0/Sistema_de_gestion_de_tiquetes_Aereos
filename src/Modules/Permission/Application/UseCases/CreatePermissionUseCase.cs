namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
public sealed class CreatePermissionUseCase
{
    private readonly IPermissionService _service;
    public CreatePermissionUseCase(IPermissionService service) => _service = service;
    public Task<PermissionDto> ExecuteAsync(CreatePermissionRequest request, CancellationToken ct = default)
        => _service.CreateAsync(request, ct);
}
