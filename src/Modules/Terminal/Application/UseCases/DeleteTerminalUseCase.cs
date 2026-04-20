namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;

/// <summary>Caso de uso: Eliminar terminal.</summary>
public sealed class DeleteTerminalUseCase
{
    private readonly ITerminalService _service;
    public DeleteTerminalUseCase(ITerminalService service) => _service = service;

    public Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _service.DeleteAsync(id, cancellationToken);
}
