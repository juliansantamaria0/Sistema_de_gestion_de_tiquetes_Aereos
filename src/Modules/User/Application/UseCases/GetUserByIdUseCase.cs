namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
public sealed class GetUserByIdUseCase
{
    private readonly IUserService _service;
    public GetUserByIdUseCase(IUserService service) => _service = service;
    public Task<UserDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
