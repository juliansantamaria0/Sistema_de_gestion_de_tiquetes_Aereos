namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class AirlineConsoleUI : ReflectiveModuleUI<IAirlineService>
{
    public AirlineConsoleUI(IAirlineService service, IServiceProvider serviceProvider)
        : base("airline", "Airline Management", service, serviceProvider)
    {
    }
}
