namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;

public interface IRoleService
{
    Task<RoleDto>               CreateAsync(CreateRoleRequest  request, CancellationToken ct = default);
    Task<RoleDto?>              GetByIdAsync(int id,                    CancellationToken ct = default);
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct = default);
    Task<RoleDto>               UpdateAsync(int id, UpdateRoleRequest request, CancellationToken ct = default);
    Task                        DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record RoleDto(int RoleId, string Name, bool IsActive);
public sealed record CreateRoleRequest(string Name, bool IsActive = true);
public sealed record UpdateRoleRequest(string Name, bool IsActive);
