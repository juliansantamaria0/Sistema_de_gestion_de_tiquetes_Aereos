namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CityConsoleUI : ReflectiveModuleUI<ICityService>
{
    public CityConsoleUI(ICityService service, IServiceProvider serviceProvider)
        : base("city", "City Management", service, serviceProvider)
    {
    }
}
