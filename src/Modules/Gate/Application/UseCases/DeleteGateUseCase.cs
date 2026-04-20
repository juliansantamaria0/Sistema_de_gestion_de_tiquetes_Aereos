namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;

/// <summary>Caso de uso: Eliminar puerta de embarque.</summary>
public sealed class DeleteGateUseCase
{
    private readonly IGateService _service;
    public DeleteGateUseCase(IGateService service) => _service = service;

    public Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _service.DeleteAsync(id, cancellationToken);
}
