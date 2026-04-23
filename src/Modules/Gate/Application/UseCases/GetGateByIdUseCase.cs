namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;


public sealed class GetGateByIdUseCase
{
    private readonly IGateService _service;
    public GetGateByIdUseCase(IGateService service) => _service = service;

    public Task<GateDto?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _service.GetByIdAsync(id, cancellationToken);
}
