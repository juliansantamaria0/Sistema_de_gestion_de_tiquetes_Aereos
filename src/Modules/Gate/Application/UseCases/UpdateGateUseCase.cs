namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;


public sealed class UpdateGateUseCase
{
    private readonly IGateService _service;
    public UpdateGateUseCase(IGateService service) => _service = service;

    public Task<GateDto> ExecuteAsync(int id, UpdateGateRequest request, CancellationToken cancellationToken = default)
        => _service.UpdateAsync(id, request, cancellationToken);
}
