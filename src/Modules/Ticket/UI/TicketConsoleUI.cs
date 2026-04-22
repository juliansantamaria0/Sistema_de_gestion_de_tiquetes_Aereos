namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class TicketConsoleUI : ReflectiveModuleUI<ITicketService>
{
    public TicketConsoleUI(ITicketService service, IServiceProvider serviceProvider)
        : base("ticket", "Ticket Management", service, serviceProvider)
    {
    }
}
