namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
public sealed class GetJobPositionByIdUseCase
{
    private readonly IJobPositionService _service;
    public GetJobPositionByIdUseCase(IJobPositionService service) => _service = service;
    public Task<JobPositionDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
