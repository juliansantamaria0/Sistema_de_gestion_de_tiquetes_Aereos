namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CustomerConsoleUI : ReflectiveModuleUI<ICustomerService>
{
    public CustomerConsoleUI(ICustomerService service, IServiceProvider serviceProvider)
        : base("customer", "Customer Management", service, serviceProvider)
    {
    }
}
