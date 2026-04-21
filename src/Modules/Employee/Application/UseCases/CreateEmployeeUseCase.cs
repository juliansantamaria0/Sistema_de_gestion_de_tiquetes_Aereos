namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
public sealed class CreateEmployeeUseCase
{
    private readonly IEmployeeService _service;
    public CreateEmployeeUseCase(IEmployeeService service) => _service = service;
    public Task<EmployeeDto> ExecuteAsync(CreateEmployeeRequest request, CancellationToken ct = default)
        => _service.CreateAsync(request, ct);
}
