namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CabinClassConsoleUI : ReflectiveModuleUI<ICabinClassService>
{
    public CabinClassConsoleUI(ICabinClassService service, IServiceProvider serviceProvider)
        : base("cabin_class", "Cabin Class Management", service, serviceProvider)
    {
    }
}
