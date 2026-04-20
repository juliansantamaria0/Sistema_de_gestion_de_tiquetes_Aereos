namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

/// <summary>Repositorio EF Core para Terminal.</summary>
public sealed class TerminalRepository : ITerminalRepository
{
    private readonly AppDbContext _context;
    public TerminalRepository(AppDbContext context) => _context = context;

    public async Task<TerminalAggregate?> GetByIdAsync(TerminalId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Terminals
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.TerminalId == id.Value, cancellationToken);
        return entity is null ? null : MapToAggregate(entity);
    }

    public async Task<IReadOnlyList<TerminalAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Terminals
            .AsNoTracking()
            .OrderBy(e => e.AirportId).ThenBy(e => e.Name)
            .ToListAsync(cancellationToken);
        return entities.Select(MapToAggregate).ToList();
    }

    public async Task AddAsync(TerminalAggregate terminal, CancellationToken cancellationToken = default)
        => await _context.Terminals.AddAsync(MapToEntity(terminal), cancellationToken);

    public void Update(TerminalAggregate terminal)
        => _context.Terminals.Update(MapToEntity(terminal));

    public void Delete(TerminalAggregate terminal)
        => _context.Terminals.Remove(new TerminalEntity { TerminalId = terminal.Id.Value });

    private static TerminalAggregate MapToAggregate(TerminalEntity e) =>
        TerminalAggregate.Reconstitute(e.TerminalId, e.AirportId, e.Name, e.IsInternational, e.CreatedAt);

    private static TerminalEntity MapToEntity(TerminalAggregate a) => new()
    {
        TerminalId      = a.Id.Value,
        AirportId       = a.AirportId,
        Name            = a.Name,
        IsInternational = a.IsInternational,
        CreatedAt       = a.CreatedAt
    };
}
