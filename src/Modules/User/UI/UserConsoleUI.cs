namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class UserConsoleUI : ReflectiveModuleUI<IUserService>
{
    public UserConsoleUI(IUserService service, IServiceProvider serviceProvider)
        : base("user", "User Management", service, serviceProvider)
    {
    }
}
