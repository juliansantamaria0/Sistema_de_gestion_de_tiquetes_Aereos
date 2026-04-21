namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.ValueObject;

public interface INationalityRepository
{
    Task<NationalityAggregate?>          GetByIdAsync(NationalityId id, CancellationToken ct = default);
    Task<IReadOnlyList<NationalityAggregate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(NationalityAggregate entity, CancellationToken ct = default);
    void Update(NationalityAggregate entity);
    void Delete(NationalityAggregate entity);
}
