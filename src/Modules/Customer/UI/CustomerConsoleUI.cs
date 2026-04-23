using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.Customer.UI;

public sealed class CustomerConsoleUI : ReflectiveModuleUI<ICustomerService>
{
    public CustomerConsoleUI(ICustomerService service, IServiceProvider serviceProvider)
        : base("Customer", "Clientes", service, serviceProvider)
    {
    }
}