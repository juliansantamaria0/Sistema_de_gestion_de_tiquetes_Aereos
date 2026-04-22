namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightCabinPriceConsoleUI : ReflectiveModuleUI<IFlightCabinPriceService>
{
    public FlightCabinPriceConsoleUI(IFlightCabinPriceService service, IServiceProvider serviceProvider)
        : base("flight_cabin_price", "Flight Cabin Price Management", service, serviceProvider)
    {
    }
}
