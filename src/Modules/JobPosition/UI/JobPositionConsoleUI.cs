
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.JobPosition.UI;

public sealed class JobPositionConsoleUI : ReflectiveModuleUI<IJobPositionService>
{
    public JobPositionConsoleUI(IJobPositionService service, IServiceProvider serviceProvider)
        : base("JobPosition", "Cargos", service, serviceProvider)
    {
    }
}