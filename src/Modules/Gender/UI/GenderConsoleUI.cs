using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.Gender.UI;

public sealed class GenderConsoleUI : ReflectiveModuleUI<IGenderService>
{
    public GenderConsoleUI(IGenderService service, IServiceProvider serviceProvider)
        : base("Gender", "Géneros", service, serviceProvider)
    {
    }
}