namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;

public interface IPassengerContactRepository
{
    Task<PassengerContactAggregate?>             GetByIdAsync(PassengerContactId id,                      CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerContactAggregate>> GetAllAsync(                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerContactAggregate>> GetByPassengerAsync(int passengerId,                     CancellationToken cancellationToken = default);
    Task                                         AddAsync(PassengerContactAggregate contact,             CancellationToken cancellationToken = default);
    Task                                         UpdateAsync(PassengerContactAggregate contact,          CancellationToken cancellationToken = default);
    Task                                         DeleteAsync(PassengerContactId id,                      CancellationToken cancellationToken = default);
}
