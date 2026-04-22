namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class EmployeeConsoleUI : ReflectiveModuleUI<IEmployeeService>
{
    public EmployeeConsoleUI(IEmployeeService service, IServiceProvider serviceProvider)
        : base("employee", "Employee Management", service, serviceProvider)
    {
    }
}
