namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class AirportConsoleUI : ReflectiveModuleUI<IAirportService>
{
    public AirportConsoleUI(IAirportService service, IServiceProvider serviceProvider)
        : base("airport", "Airport Management", service, serviceProvider)
    {
    }
}
