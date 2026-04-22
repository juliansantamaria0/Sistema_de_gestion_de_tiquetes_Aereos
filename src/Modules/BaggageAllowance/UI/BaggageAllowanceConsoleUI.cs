namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class BaggageAllowanceConsoleUI : ReflectiveModuleUI<IBaggageAllowanceService>
{
    public BaggageAllowanceConsoleUI(IBaggageAllowanceService service, IServiceProvider serviceProvider)
        : base("baggage_allowance", "Baggage Allowance Management", service, serviceProvider)
    {
    }
}
