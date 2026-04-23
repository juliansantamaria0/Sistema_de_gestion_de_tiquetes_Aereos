using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.Nationality.UI;

public sealed class NationalityConsoleUI : ReflectiveModuleUI<INationalityService>
{
    public NationalityConsoleUI(INationalityService service, IServiceProvider serviceProvider)
        : base("Nationality", "Nacionalidades", service, serviceProvider)
    {
    }
}