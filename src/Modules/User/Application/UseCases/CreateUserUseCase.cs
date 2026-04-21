namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
public sealed class CreateUserUseCase
{
    private readonly IUserService _service;
    public CreateUserUseCase(IUserService service) => _service = service;
    public Task<UserDto> ExecuteAsync(CreateUserRequest request, CancellationToken ct = default)
        => _service.CreateAsync(request, ct);
}
