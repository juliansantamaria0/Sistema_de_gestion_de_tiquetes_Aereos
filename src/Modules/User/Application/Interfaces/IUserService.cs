namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;

public interface IUserService
{
    Task<UserDto>               CreateAsync(CreateUserRequest  request, CancellationToken ct = default);
    Task<UserDto?>              GetByIdAsync(int id,                    CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserDto>               UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default);
    Task                        DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record UserDto(
    int      UserId,
    int      PersonId,
    int      RoleId,
    string   Username,
    bool     IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);


public sealed record CreateUserRequest(int PersonId, int RoleId, string Username, string Password, bool IsActive = true);
public sealed record UpdateUserRequest(int RoleId, string Username, bool IsActive);
