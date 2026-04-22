namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Currency.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Currency.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CurrencyConsoleUI : ReflectiveModuleUI<ICurrencyService>
{
    public CurrencyConsoleUI(ICurrencyService service, IServiceProvider serviceProvider)
        : base("currency", "Currency Management", service, serviceProvider)
    {
    }
}
