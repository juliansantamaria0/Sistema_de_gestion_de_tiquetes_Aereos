namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;

public interface IPersonRepository
{
    Task<PersonAggregate?>             GetByIdAsync(PersonId id,                                       CancellationToken cancellationToken = default);
    Task<IEnumerable<PersonAggregate>> GetAllAsync(                                                    CancellationToken cancellationToken = default);
    Task<PersonAggregate?>             GetByDocumentAsync(int documentTypeId, string documentNumber,  CancellationToken cancellationToken = default);
    Task                               AddAsync(PersonAggregate person,                               CancellationToken cancellationToken = default);
    Task                               UpdateAsync(PersonAggregate person,                            CancellationToken cancellationToken = default);
    Task                               DeleteAsync(PersonId id,                                       CancellationToken cancellationToken = default);
}
