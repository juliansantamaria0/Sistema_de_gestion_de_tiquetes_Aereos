namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightStatusConsoleUI : ReflectiveModuleUI<IFlightStatusService>
{
    public FlightStatusConsoleUI(IFlightStatusService service, IServiceProvider serviceProvider)
        : base("flight_status", "Flight Status Management", service, serviceProvider)
    {
    }
}
