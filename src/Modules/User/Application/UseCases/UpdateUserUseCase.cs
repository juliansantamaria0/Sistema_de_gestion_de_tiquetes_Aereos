namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
public sealed class UpdateUserUseCase
{
    private readonly IUserService _service;
    public UpdateUserUseCase(IUserService service) => _service = service;
    public Task<UserDto> ExecuteAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
        => _service.UpdateAsync(id, request, ct);
}
