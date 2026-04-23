namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;


public sealed class CreateTerminalUseCase
{
    private readonly ITerminalService _service;
    public CreateTerminalUseCase(ITerminalService service) => _service = service;

    public Task<TerminalDto> ExecuteAsync(CreateTerminalRequest request, CancellationToken cancellationToken = default)
        => _service.CreateAsync(request, cancellationToken);
}
