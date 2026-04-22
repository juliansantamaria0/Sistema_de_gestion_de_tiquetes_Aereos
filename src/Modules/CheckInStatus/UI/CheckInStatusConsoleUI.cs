namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CheckInStatusConsoleUI : ReflectiveModuleUI<ICheckInStatusService>
{
    public CheckInStatusConsoleUI(ICheckInStatusService service, IServiceProvider serviceProvider)
        : base("check_in_status", "Check In Status Management", service, serviceProvider)
    {
    }
}
