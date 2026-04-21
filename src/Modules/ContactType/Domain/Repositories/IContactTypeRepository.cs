namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.ValueObject;

public interface IContactTypeRepository
{
    Task<ContactTypeAggregate?>             GetByIdAsync(ContactTypeId id,                 CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactTypeAggregate>> GetAllAsync(                                    CancellationToken cancellationToken = default);
    Task                                    AddAsync(ContactTypeAggregate contactType,     CancellationToken cancellationToken = default);
    Task                                    UpdateAsync(ContactTypeAggregate contactType,  CancellationToken cancellationToken = default);
    Task                                    DeleteAsync(ContactTypeId id,                  CancellationToken cancellationToken = default);
}
