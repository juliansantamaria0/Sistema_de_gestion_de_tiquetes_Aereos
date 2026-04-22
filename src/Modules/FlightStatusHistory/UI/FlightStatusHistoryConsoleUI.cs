namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightStatusHistoryConsoleUI : ReflectiveModuleUI<IFlightStatusHistoryService>
{
    public FlightStatusHistoryConsoleUI(IFlightStatusHistoryService service, IServiceProvider serviceProvider)
        : base("flight_status_history", "Flight Status History Management", service, serviceProvider)
    {
    }
}
