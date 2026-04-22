namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PromotionConsoleUI : ReflectiveModuleUI<IPromotionService>
{
    public PromotionConsoleUI(IPromotionService service, IServiceProvider serviceProvider)
        : base("promotion", "Promotion Management", service, serviceProvider)
    {
    }
}
