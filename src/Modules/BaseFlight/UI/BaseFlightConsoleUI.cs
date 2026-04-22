namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class BaseFlightConsoleUI : ReflectiveModuleUI<IBaseFlightService>
{
    public BaseFlightConsoleUI(IBaseFlightService service, IServiceProvider serviceProvider)
        : base("base_flight", "Base Flight Management", service, serviceProvider)
    {
    }
}
