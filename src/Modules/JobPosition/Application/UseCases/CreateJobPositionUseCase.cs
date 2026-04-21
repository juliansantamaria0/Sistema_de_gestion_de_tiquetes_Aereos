namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
public sealed class CreateJobPositionUseCase
{
    private readonly IJobPositionService _service;
    public CreateJobPositionUseCase(IJobPositionService service) => _service = service;
    public Task<JobPositionDto> ExecuteAsync(CreateJobPositionRequest request, CancellationToken ct = default)
        => _service.CreateAsync(request, ct);
}
