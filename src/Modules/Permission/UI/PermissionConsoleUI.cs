namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PermissionConsoleUI : ReflectiveModuleUI<IPermissionService>
{
    public PermissionConsoleUI(IPermissionService service, IServiceProvider serviceProvider)
        : base("permission", "Permission Management", service, serviceProvider)
    {
    }
}
