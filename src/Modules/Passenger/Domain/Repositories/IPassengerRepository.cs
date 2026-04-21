namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;

public interface IPassengerRepository
{
    Task<PassengerAggregate?>             GetByIdAsync(PassengerId id,                         CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerAggregate>> GetAllAsync(                                          CancellationToken cancellationToken = default);
    Task<PassengerAggregate?>             GetByPersonAsync(int personId,                       CancellationToken cancellationToken = default);
    Task                                  AddAsync(PassengerAggregate passenger,               CancellationToken cancellationToken = default);
    Task                                  UpdateAsync(PassengerAggregate passenger,            CancellationToken cancellationToken = default);
    Task                                  DeleteAsync(PassengerId id,                          CancellationToken cancellationToken = default);
}
