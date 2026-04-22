namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CountryConsoleUI : ReflectiveModuleUI<ICountryService>
{
    public CountryConsoleUI(ICountryService service, IServiceProvider serviceProvider)
        : base("country", "Country Management", service, serviceProvider)
    {
    }
}
