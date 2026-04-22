namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Aircraft.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Aircraft.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class AircraftConsoleUI : ReflectiveModuleUI<IAircraftService>
{
    public AircraftConsoleUI(IAircraftService service, IServiceProvider serviceProvider)
        : base("aircraft", "Aircraft Management", service, serviceProvider)
    {
    }
}
