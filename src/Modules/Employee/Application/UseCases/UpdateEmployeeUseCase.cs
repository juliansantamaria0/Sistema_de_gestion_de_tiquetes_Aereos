namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
public sealed class UpdateEmployeeUseCase
{
    private readonly IEmployeeService _service;
    public UpdateEmployeeUseCase(IEmployeeService service) => _service = service;
    public Task<EmployeeDto> ExecuteAsync(int id, UpdateEmployeeRequest request, CancellationToken ct = default)
        => _service.UpdateAsync(id, request, ct);
}
