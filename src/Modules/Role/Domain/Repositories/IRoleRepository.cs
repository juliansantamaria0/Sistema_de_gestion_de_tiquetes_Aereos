namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.ValueObject;

public interface IRoleRepository
{
    Task<RoleAggregate?>          GetByIdAsync(RoleId id, CancellationToken ct = default);
    Task<IReadOnlyList<RoleAggregate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(RoleAggregate entity, CancellationToken ct = default);
    void Update(RoleAggregate entity);
    void Delete(RoleAggregate entity);
}
