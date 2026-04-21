namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDto>               CreateAsync(CreateEmployeeRequest  request, CancellationToken ct = default);
    Task<EmployeeDto?>              GetByIdAsync(int id,                        CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken ct = default);
    Task<EmployeeDto>               UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct = default);
    Task                            DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record EmployeeDto(
    int       EmployeeId,
    int       PersonId,
    int       AirlineId,
    int?      JobPositionId,
    DateOnly  HireDate,
    bool      IsActive,
    DateTime  CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreateEmployeeRequest(
    int      PersonId,
    int      AirlineId,
    DateOnly HireDate,
    int?     JobPositionId = null,
    bool     IsActive      = true);

public sealed record UpdateEmployeeRequest(
    int      AirlineId,
    int?     JobPositionId,
    DateOnly HireDate,
    bool     IsActive);
