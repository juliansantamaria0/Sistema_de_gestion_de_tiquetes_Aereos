namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;
    public EmployeeRepository(AppDbContext context) => _context = context;

    public async Task<EmployeeAggregate?> GetByIdAsync(EmployeeId id, CancellationToken ct = default)
    {
        var e = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmployeeId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<EmployeeAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.Employees
            .AsNoTracking()
            .OrderBy(x => x.EmployeeId)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task AddAsync(EmployeeAggregate entity, CancellationToken ct = default)
        => await _context.Employees.AddAsync(ToEntity(entity), ct);

    public void Update(EmployeeAggregate entity) => _context.Employees.Update(ToEntity(entity));

    public void Delete(EmployeeAggregate entity)
        => _context.Employees.Remove(new EmployeeEntity { EmployeeId = entity.Id.Value });

    private static EmployeeAggregate ToAggregate(EmployeeEntity e) =>
        EmployeeAggregate.Reconstitute(e.EmployeeId, e.PersonId, e.AirlineId,
            e.JobPositionId, e.HireDate, e.IsActive, e.CreatedAt, e.UpdatedAt);

    private static EmployeeEntity ToEntity(EmployeeAggregate a) => new()
    {
        EmployeeId    = a.Id.Value,
        PersonId      = a.PersonId,
        AirlineId     = a.AirlineId,
        JobPositionId = a.JobPositionId,
        HireDate      = a.HireDate,
        IsActive      = a.IsActive,
        CreatedAt     = a.CreatedAt,
        UpdatedAt     = a.UpdatedAt
    };
}
