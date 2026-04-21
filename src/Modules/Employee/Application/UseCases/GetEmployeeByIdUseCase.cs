namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
public sealed class GetEmployeeByIdUseCase
{
    private readonly IEmployeeService _service;
    public GetEmployeeByIdUseCase(IEmployeeService service) => _service = service;
    public Task<EmployeeDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
