namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class GenderConsoleUI : ReflectiveModuleUI<IGenderService>
{
    public GenderConsoleUI(IGenderService service, IServiceProvider serviceProvider)
        : base("gender", "Gender Management", service, serviceProvider)
    {
    }
}
