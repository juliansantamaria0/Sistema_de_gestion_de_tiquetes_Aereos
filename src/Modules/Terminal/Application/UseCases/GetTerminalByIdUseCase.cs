namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;


public sealed class GetTerminalByIdUseCase
{
    private readonly ITerminalService _service;
    public GetTerminalByIdUseCase(ITerminalService service) => _service = service;

    public Task<TerminalDto?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _service.GetByIdAsync(id, cancellationToken);
}
