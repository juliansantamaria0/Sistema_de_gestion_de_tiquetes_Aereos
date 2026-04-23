namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;


public sealed class GateRepository : IGateRepository
{
    private readonly AppDbContext _context;
    public GateRepository(AppDbContext context) => _context = context;

    public async Task<GateAggregate?> GetByIdAsync(GateId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Gates
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.GateId == id.Value, cancellationToken);
        return entity is null ? null : MapToAggregate(entity);
    }

    public async Task<IReadOnlyList<GateAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Gates
            .AsNoTracking()
            .OrderBy(e => e.TerminalId).ThenBy(e => e.Code)
            .ToListAsync(cancellationToken);
        return entities.Select(MapToAggregate).ToList();
    }

    public async Task AddAsync(GateAggregate gate, CancellationToken cancellationToken = default)
        => await _context.Gates.AddAsync(MapToEntity(gate), cancellationToken);

    public void Update(GateAggregate gate)
        => _context.Gates.Update(MapToEntity(gate));

    public void Delete(GateAggregate gate)
        => _context.Gates.Remove(new GateEntity { GateId = gate.Id.Value });

    private static GateAggregate MapToAggregate(GateEntity e) =>
        GateAggregate.Reconstitute(e.GateId, e.TerminalId, e.Code, e.IsActive);

    private static GateEntity MapToEntity(GateAggregate a) => new()
    {
        GateId     = a.Id.Value,
        TerminalId = a.TerminalId,
        Code       = a.Code,
        IsActive   = a.IsActive
    };
}
