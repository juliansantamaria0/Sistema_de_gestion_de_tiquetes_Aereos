using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.UI;

public sealed class ReprogrammingHistoryConsoleUI : ReflectiveModuleUI<IReprogrammingHistoryService>
{
    public ReprogrammingHistoryConsoleUI(IReprogrammingHistoryService service, IServiceProvider serviceProvider)
        : base("ReprogrammingHistory", "Historial de reprogramaciones", service, serviceProvider)
    {
    }
}

