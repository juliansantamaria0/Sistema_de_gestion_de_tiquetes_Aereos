namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
public sealed class DeleteEmployeeUseCase
{
    private readonly IEmployeeService _service;
    public DeleteEmployeeUseCase(IEmployeeService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
