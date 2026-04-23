namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.ValueObject;


public interface ITerminalRepository
{
    Task<TerminalAggregate?> GetByIdAsync(TerminalId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TerminalAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TerminalAggregate terminal, CancellationToken cancellationToken = default);
    void Update(TerminalAggregate terminal);
    void Delete(TerminalAggregate terminal);
}
