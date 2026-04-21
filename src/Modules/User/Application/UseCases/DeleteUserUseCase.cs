namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
public sealed class DeleteUserUseCase
{
    private readonly IUserService _service;
    public DeleteUserUseCase(IUserService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
