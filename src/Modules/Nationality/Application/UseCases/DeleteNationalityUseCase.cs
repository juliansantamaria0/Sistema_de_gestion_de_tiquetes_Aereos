namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;

public sealed class DeleteNationalityUseCase
{
    private readonly INationalityService _service;
    public DeleteNationalityUseCase(INationalityService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
