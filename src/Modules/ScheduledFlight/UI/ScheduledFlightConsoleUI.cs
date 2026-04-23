using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.ScheduledFlight.UI;

public sealed class ScheduledFlightConsoleUI : ReflectiveModuleUI<IScheduledFlightService>
{
    public ScheduledFlightConsoleUI(IScheduledFlightService service, IServiceProvider serviceProvider)
        : base("ScheduledFlight", "Vuelos programados", service, serviceProvider)
    {
    }
}