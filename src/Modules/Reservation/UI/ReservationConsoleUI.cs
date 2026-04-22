namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class ReservationConsoleUI : ReflectiveModuleUI<IReservationService>
{
    public ReservationConsoleUI(IReservationService service, IServiceProvider serviceProvider)
        : base("reservation", "Reservation Management", service, serviceProvider)
    {
    }
}
