namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class SeatStatusConsoleUI : ReflectiveModuleUI<ISeatStatusService>
{
    public SeatStatusConsoleUI(ISeatStatusService service, IServiceProvider serviceProvider)
        : base("seat_status", "Seat Status Management", service, serviceProvider)
    {
    }
}
