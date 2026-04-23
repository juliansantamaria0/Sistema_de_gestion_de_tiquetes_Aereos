namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.ValueObject;


public interface IGateRepository
{
    Task<GateAggregate?> GetByIdAsync(GateId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GateAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(GateAggregate gate, CancellationToken cancellationToken = default);
    void Update(GateAggregate gate);
    void Delete(GateAggregate gate);
}
