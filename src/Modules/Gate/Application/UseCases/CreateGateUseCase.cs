namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;


public sealed class CreateGateUseCase
{
    private readonly IGateService _service;
    public CreateGateUseCase(IGateService service) => _service = service;

    public Task<GateDto> ExecuteAsync(CreateGateRequest request, CancellationToken cancellationToken = default)
        => _service.CreateAsync(request, cancellationToken);
}
