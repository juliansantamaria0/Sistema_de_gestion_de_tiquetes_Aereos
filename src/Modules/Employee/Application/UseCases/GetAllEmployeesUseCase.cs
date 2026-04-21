namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
public sealed class GetAllEmployeesUseCase
{
    private readonly IEmployeeService _service;
    public GetAllEmployeesUseCase(IEmployeeService service) => _service = service;
    public Task<IReadOnlyList<EmployeeDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
