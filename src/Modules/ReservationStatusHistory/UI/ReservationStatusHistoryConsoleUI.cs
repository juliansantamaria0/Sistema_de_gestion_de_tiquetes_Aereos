namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class ReservationStatusHistoryConsoleUI : ReflectiveModuleUI<IReservationStatusHistoryService>
{
    public ReservationStatusHistoryConsoleUI(IReservationStatusHistoryService service, IServiceProvider serviceProvider)
        : base("reservation_status_history", "Reservation Status History Management", service, serviceProvider)
    {
    }
}
