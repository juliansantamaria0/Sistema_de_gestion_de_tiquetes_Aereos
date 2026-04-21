namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.ValueObject;

public interface IJobPositionRepository
{
    Task<JobPositionAggregate?>          GetByIdAsync(JobPositionId id, CancellationToken ct = default);
    Task<IReadOnlyList<JobPositionAggregate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(JobPositionAggregate entity, CancellationToken ct = default);
    void Update(JobPositionAggregate entity);
    void Delete(JobPositionAggregate entity);
}
