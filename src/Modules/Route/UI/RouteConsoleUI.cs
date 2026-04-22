namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RouteConsoleUI : ReflectiveModuleUI<IRouteService>
{
    public RouteConsoleUI(IRouteService service, IServiceProvider serviceProvider)
        : base("route", "Route Management", service, serviceProvider)
    {
    }
}
