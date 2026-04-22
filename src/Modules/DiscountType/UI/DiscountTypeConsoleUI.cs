namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class DiscountTypeConsoleUI : ReflectiveModuleUI<IDiscountTypeService>
{
    public DiscountTypeConsoleUI(IDiscountTypeService service, IServiceProvider serviceProvider)
        : base("discount_type", "Discount Type Management", service, serviceProvider)
    {
    }
}
