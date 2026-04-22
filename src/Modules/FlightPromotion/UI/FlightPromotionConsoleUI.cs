namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlightPromotionConsoleUI : ReflectiveModuleUI<IFlightPromotionService>
{
    public FlightPromotionConsoleUI(IFlightPromotionService service, IServiceProvider serviceProvider)
        : base("flight_promotion", "Flight Promotion Management", service, serviceProvider)
    {
    }
}
