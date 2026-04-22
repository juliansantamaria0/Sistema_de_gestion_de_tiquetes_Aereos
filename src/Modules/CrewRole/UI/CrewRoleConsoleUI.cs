namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CrewRole.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CrewRole.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CrewRoleConsoleUI : ReflectiveModuleUI<ICrewRoleService>
{
    public CrewRoleConsoleUI(ICrewRoleService service, IServiceProvider serviceProvider)
        : base("crew_role", "Crew Role Management", service, serviceProvider)
    {
    }
}
