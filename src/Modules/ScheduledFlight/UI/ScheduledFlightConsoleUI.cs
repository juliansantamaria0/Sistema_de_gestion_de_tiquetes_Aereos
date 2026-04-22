namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class ScheduledFlightConsoleUI : ReflectiveModuleUI<IScheduledFlightService>
{
    public ScheduledFlightConsoleUI(IScheduledFlightService service, IServiceProvider serviceProvider)
        : base("scheduled_flight", "Scheduled Flight Management", service, serviceProvider)
    {
    }
}
