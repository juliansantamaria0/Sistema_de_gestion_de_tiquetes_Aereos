namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
public sealed class GetAllJobPositionsUseCase
{
    private readonly IJobPositionService _service;
    public GetAllJobPositionsUseCase(IJobPositionService service) => _service = service;
    public Task<IReadOnlyList<JobPositionDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
