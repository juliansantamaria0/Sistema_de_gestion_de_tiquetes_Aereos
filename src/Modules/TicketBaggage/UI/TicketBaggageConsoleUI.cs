namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class TicketBaggageConsoleUI : ReflectiveModuleUI<ITicketBaggageService>
{
    public TicketBaggageConsoleUI(ITicketBaggageService service, IServiceProvider serviceProvider)
        : base("ticket_baggage", "Ticket Baggage Management", service, serviceProvider)
    {
    }
}
