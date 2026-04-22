namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class JobPositionConsoleUI : ReflectiveModuleUI<IJobPositionService>
{
    public JobPositionConsoleUI(IJobPositionService service, IServiceProvider serviceProvider)
        : base("job_position", "Job Position Management", service, serviceProvider)
    {
    }
}
