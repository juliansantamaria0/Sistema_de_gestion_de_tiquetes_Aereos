namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class TicketStatusHistoryConsoleUI : ReflectiveModuleUI<ITicketStatusHistoryService>
{
    public TicketStatusHistoryConsoleUI(ITicketStatusHistoryService service, IServiceProvider serviceProvider)
        : base("ticket_status_history", "Ticket Status History Management", service, serviceProvider)
    {
    }
}
