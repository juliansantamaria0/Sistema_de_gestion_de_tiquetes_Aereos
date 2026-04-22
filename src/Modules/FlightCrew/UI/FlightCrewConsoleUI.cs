namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightCrewConsoleUI : ReflectiveModuleUI<IFlightCrewService>
{
    public FlightCrewConsoleUI(IFlightCrewService service, IServiceProvider serviceProvider)
        : base("flight_crew", "Flight Crew Management", service, serviceProvider)
    {
    }
}
