namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightCancellationConsoleUI : ReflectiveModuleUI<IFlightCancellationService>
{
    public FlightCancellationConsoleUI(IFlightCancellationService service, IServiceProvider serviceProvider)
        : base("flight_cancellation", "Flight Cancellation Management", service, serviceProvider)
    {
    }
}
