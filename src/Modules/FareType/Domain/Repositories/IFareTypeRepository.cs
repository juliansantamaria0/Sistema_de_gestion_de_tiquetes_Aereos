namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;

public interface IFareTypeRepository
{
    Task<FareTypeAggregate?>             GetByIdAsync(FareTypeId id,               CancellationToken cancellationToken = default);
    Task<IEnumerable<FareTypeAggregate>> GetAllAsync(                               CancellationToken cancellationToken = default);
    Task                                 AddAsync(FareTypeAggregate fareType,      CancellationToken cancellationToken = default);
    Task                                 UpdateAsync(FareTypeAggregate fareType,   CancellationToken cancellationToken = default);
    Task                                 DeleteAsync(FareTypeId id,                CancellationToken cancellationToken = default);
}
