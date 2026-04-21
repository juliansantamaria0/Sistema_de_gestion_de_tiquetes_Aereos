namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.ValueObject;

public interface IUserRepository
{
    Task<UserAggregate?>          GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<IReadOnlyList<UserAggregate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(UserAggregate entity, CancellationToken ct = default);
    void Update(UserAggregate entity);
    void Delete(UserAggregate entity);
}
