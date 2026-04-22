namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class BoardingPassConsoleUI : ReflectiveModuleUI<IBoardingPassService>
{
    public BoardingPassConsoleUI(IBoardingPassService service, IServiceProvider serviceProvider)
        : base("boarding_pass", "Boarding Pass Management", service, serviceProvider)
    {
    }
}
