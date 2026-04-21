namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class JobPositionRepository : IJobPositionRepository
{
    private readonly AppDbContext _context;
    public JobPositionRepository(AppDbContext context) => _context = context;

    public async Task<JobPositionAggregate?> GetByIdAsync(JobPositionId id, CancellationToken ct = default)
    {
        var e = await _context.JobPositions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobPositionId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<JobPositionAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.JobPositions
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task AddAsync(JobPositionAggregate entity, CancellationToken ct = default)
        => await _context.JobPositions.AddAsync(ToEntity(entity), ct);

    public void Update(JobPositionAggregate entity) => _context.JobPositions.Update(ToEntity(entity));

    public void Delete(JobPositionAggregate entity)
        => _context.JobPositions.Remove(new JobPositionEntity { JobPositionId = entity.Id.Value });

    private static JobPositionAggregate ToAggregate(JobPositionEntity e) =>
        JobPositionAggregate.Reconstitute(e.JobPositionId, e.Name, e.Department);

    private static JobPositionEntity ToEntity(JobPositionAggregate a) =>
        new() { JobPositionId = a.Id.Value, Name = a.Name, Department = a.Department };
}
