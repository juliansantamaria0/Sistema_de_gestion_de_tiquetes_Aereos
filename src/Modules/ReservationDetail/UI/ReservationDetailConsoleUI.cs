namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class ReservationDetailConsoleUI : ReflectiveModuleUI<IReservationDetailService>
{
    public ReservationDetailConsoleUI(IReservationDetailService service, IServiceProvider serviceProvider)
        : base("reservation_detail", "Reservation Detail Management", service, serviceProvider)
    {
    }
}
