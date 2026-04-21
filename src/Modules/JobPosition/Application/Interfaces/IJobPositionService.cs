namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;

public interface IJobPositionService
{
    Task<JobPositionDto>               CreateAsync(CreateJobPositionRequest  request, CancellationToken ct = default);
    Task<JobPositionDto?>              GetByIdAsync(int id,                           CancellationToken ct = default);
    Task<IReadOnlyList<JobPositionDto>> GetAllAsync(CancellationToken ct = default);
    Task<JobPositionDto>               UpdateAsync(int id, UpdateJobPositionRequest request, CancellationToken ct = default);
    Task                               DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record JobPositionDto(int JobPositionId, string Name, string? Department);
public sealed record CreateJobPositionRequest(string Name, string? Department = null);
public sealed record UpdateJobPositionRequest(string Name, string? Department);
