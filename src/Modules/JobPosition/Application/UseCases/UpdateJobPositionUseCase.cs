namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
public sealed class UpdateJobPositionUseCase
{
    private readonly IJobPositionService _service;
    public UpdateJobPositionUseCase(IJobPositionService service) => _service = service;
    public Task<JobPositionDto> ExecuteAsync(int id, UpdateJobPositionRequest request, CancellationToken ct = default)
        => _service.UpdateAsync(id, request, ct);
}
