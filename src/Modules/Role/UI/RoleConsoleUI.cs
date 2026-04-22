namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RoleConsoleUI : ReflectiveModuleUI<IRoleService>
{
    public RoleConsoleUI(IRoleService service, IServiceProvider serviceProvider)
        : base("role", "Role Management", service, serviceProvider)
    {
    }
}
