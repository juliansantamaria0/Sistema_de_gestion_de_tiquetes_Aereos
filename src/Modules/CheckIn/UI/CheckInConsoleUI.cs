namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CheckInConsoleUI : ReflectiveModuleUI<ICheckInService>
{
    public CheckInConsoleUI(ICheckInService service, IServiceProvider serviceProvider)
        : base("check_in", "Check In Management", service, serviceProvider)
    {
    }
}
