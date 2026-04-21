namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;

public interface IPermissionService
{
    Task<PermissionDto>               CreateAsync(CreatePermissionRequest  request, CancellationToken ct = default);
    Task<PermissionDto?>              GetByIdAsync(int id,                          CancellationToken ct = default);
    Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken ct = default);
    Task<PermissionDto>               UpdateAsync(int id, UpdatePermissionRequest request, CancellationToken ct = default);
    Task                              DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record PermissionDto(int PermissionId, string Name, string? Description);
public sealed record CreatePermissionRequest(string Name, string? Description = null);
public sealed record UpdatePermissionRequest(string Name, string? Description);
