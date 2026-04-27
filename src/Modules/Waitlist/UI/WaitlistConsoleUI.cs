using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.UI;

public sealed class WaitlistConsoleUI : ReflectiveModuleUI<IWaitlistService>
{
    public WaitlistConsoleUI(IWaitlistService service, IServiceProvider serviceProvider)
        : base("Waitlist", "Lista de espera", service, serviceProvider)
    {
    }
}

