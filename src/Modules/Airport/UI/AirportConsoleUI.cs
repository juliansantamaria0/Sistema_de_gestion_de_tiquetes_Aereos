using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.Airport.UI;

public sealed class AirportConsoleUI : ReflectiveModuleUI<IAirportService>
{
    public AirportConsoleUI(IAirportService service, IServiceProvider serviceProvider)
        : base("Airport", "Aeropuertos", service, serviceProvider)
    {
    }
}