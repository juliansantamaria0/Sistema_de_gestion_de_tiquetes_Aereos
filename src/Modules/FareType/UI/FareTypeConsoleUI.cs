namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FareTypeConsoleUI : ReflectiveModuleUI<IFareTypeService>
{
    public FareTypeConsoleUI(IFareTypeService service, IServiceProvider serviceProvider)
        : base("faretype", "Tipos de tarifa", service, serviceProvider)
    {
    }
}
