namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;

public interface IGenderRepository
{
    Task<GenderAggregate?>             GetByIdAsync(GenderId id,              CancellationToken cancellationToken = default);
    Task<IEnumerable<GenderAggregate>> GetAllAsync(                           CancellationToken cancellationToken = default);
    Task                               AddAsync(GenderAggregate gender,       CancellationToken cancellationToken = default);
    Task                               UpdateAsync(GenderAggregate gender,    CancellationToken cancellationToken = default);
    Task                               DeleteAsync(GenderId id,               CancellationToken cancellationToken = default);
}
