namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;

/// <summary>Caso de uso: Actualizar terminal.</summary>
public sealed class UpdateTerminalUseCase
{
    private readonly ITerminalService _service;
    public UpdateTerminalUseCase(ITerminalService service) => _service = service;

    public Task<TerminalDto> ExecuteAsync(int id, UpdateTerminalRequest request, CancellationToken cancellationToken = default)
        => _service.UpdateAsync(id, request, cancellationToken);
}
