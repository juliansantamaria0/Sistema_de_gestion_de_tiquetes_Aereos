namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class BaggageTypeConsoleUI : ReflectiveModuleUI<IBaggageTypeService>
{
    public BaggageTypeConsoleUI(IBaggageTypeService service, IServiceProvider serviceProvider)
        : base("baggage_type", "Baggage Type Management", service, serviceProvider)
    {
    }
}
