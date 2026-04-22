namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class NationalityConsoleUI : ReflectiveModuleUI<INationalityService>
{
    public NationalityConsoleUI(INationalityService service, IServiceProvider serviceProvider)
        : base("nationality", "Nationality Management", service, serviceProvider)
    {
    }
}
