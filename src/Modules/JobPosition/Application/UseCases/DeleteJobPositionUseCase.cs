namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
public sealed class DeleteJobPositionUseCase
{
    private readonly IJobPositionService _service;
    public DeleteJobPositionUseCase(IJobPositionService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
