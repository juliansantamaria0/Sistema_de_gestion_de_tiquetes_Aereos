namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.ValueObject;

public interface IPermissionRepository
{
    Task<PermissionAggregate?>          GetByIdAsync(PermissionId id, CancellationToken ct = default);
    Task<IReadOnlyList<PermissionAggregate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(PermissionAggregate entity, CancellationToken ct = default);
    void Update(PermissionAggregate entity);
    void Delete(PermissionAggregate entity);
}
