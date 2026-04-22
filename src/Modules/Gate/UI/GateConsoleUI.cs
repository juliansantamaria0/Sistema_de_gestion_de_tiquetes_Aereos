namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class GateConsoleUI : ReflectiveModuleUI<IGateService>
{
    public GateConsoleUI(IGateService service, IServiceProvider serviceProvider)
        : base("gate", "Gate Management", service, serviceProvider)
    {
    }
}
