namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.ValueObject;

public interface IEmployeeRepository
{
    Task<EmployeeAggregate?>          GetByIdAsync(EmployeeId id, CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeAggregate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(EmployeeAggregate entity, CancellationToken ct = default);
    void Update(EmployeeAggregate entity);
    void Delete(EmployeeAggregate entity);
}
