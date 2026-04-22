namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightSeatConsoleUI : ReflectiveModuleUI<IFlightSeatService>
{
    public FlightSeatConsoleUI(IFlightSeatService service, IServiceProvider serviceProvider)
        : base("flight_seat", "Flight Seat Management", service, serviceProvider)
    {
    }
}
