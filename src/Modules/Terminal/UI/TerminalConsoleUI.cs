namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class TerminalConsoleUI : ReflectiveModuleUI<ITerminalService>
{
    public TerminalConsoleUI(ITerminalService service, IServiceProvider serviceProvider)
        : base("terminal", "Terminal Management", service, serviceProvider)
    {
    }
}
