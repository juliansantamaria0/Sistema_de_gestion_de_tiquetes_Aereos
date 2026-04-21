namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
public sealed class GetAllUsersUseCase
{
    private readonly IUserService _service;
    public GetAllUsersUseCase(IUserService service) => _service = service;
    public Task<IReadOnlyList<UserDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
