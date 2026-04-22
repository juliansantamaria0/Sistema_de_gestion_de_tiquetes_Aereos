namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightDelayConsoleUI : ReflectiveModuleUI<IFlightDelayService>
{
    public FlightDelayConsoleUI(IFlightDelayService service, IServiceProvider serviceProvider)
        : base("flight_delay", "Flight Delay Management", service, serviceProvider)
    {
    }
}
