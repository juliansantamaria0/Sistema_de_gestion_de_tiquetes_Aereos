namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RolePermissionConsoleUI : ReflectiveModuleUI<IRolePermissionService>
{
    public RolePermissionConsoleUI(IRolePermissionService service, IServiceProvider serviceProvider)
        : base("role_permission", "Role-Permission Assignment", service, serviceProvider)
    {
    }
}
