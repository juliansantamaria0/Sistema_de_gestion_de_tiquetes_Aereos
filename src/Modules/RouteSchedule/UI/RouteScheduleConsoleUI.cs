namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RouteScheduleConsoleUI : ReflectiveModuleUI<IRouteScheduleService>
{
    public RouteScheduleConsoleUI(IRouteScheduleService service, IServiceProvider serviceProvider)
        : base("route_schedule", "Route Schedule Management", service, serviceProvider)
    {
    }
}
