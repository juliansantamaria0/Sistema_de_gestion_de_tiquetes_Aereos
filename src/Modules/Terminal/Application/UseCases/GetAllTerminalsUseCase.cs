namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;

/// <summary>Caso de uso: Obtener todas las terminales.</summary>
public sealed class GetAllTerminalsUseCase
{
    private readonly ITerminalService _service;
    public GetAllTerminalsUseCase(ITerminalService service) => _service = service;

    public Task<IReadOnlyList<TerminalDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        => _service.GetAllAsync(cancellationToken);
}
