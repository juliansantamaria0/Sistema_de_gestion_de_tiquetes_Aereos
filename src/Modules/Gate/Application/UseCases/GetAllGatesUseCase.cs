namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;


public sealed class GetAllGatesUseCase
{
    private readonly IGateService _service;
    public GetAllGatesUseCase(IGateService service) => _service = service;

    public Task<IReadOnlyList<GateDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        => _service.GetAllAsync(cancellationToken);
}
