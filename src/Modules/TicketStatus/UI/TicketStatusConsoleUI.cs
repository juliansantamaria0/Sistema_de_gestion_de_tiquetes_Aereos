namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class TicketStatusConsoleUI : ReflectiveModuleUI<ITicketStatusService>
{
    public TicketStatusConsoleUI(ITicketStatusService service, IServiceProvider serviceProvider)
        : base("ticket_status", "Ticket Status Management", service, serviceProvider)
    {
    }
}
