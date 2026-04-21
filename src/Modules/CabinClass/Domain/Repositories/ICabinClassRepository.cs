namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.ValueObject;

public interface ICabinClassRepository
{
    Task<CabinClassAggregate?>             GetByIdAsync(CabinClassId id,               CancellationToken cancellationToken = default);
    Task<IEnumerable<CabinClassAggregate>> GetAllAsync(                                 CancellationToken cancellationToken = default);
    Task                                   AddAsync(CabinClassAggregate cabinClass,    CancellationToken cancellationToken = default);
    Task                                   UpdateAsync(CabinClassAggregate cabinClass, CancellationToken cancellationToken = default);
    Task                                   DeleteAsync(CabinClassId id,                CancellationToken cancellationToken = default);
}
