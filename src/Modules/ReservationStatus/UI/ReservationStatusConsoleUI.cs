namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class ReservationStatusConsoleUI : ReflectiveModuleUI<IReservationStatusService>
{
    public ReservationStatusConsoleUI(IReservationStatusService service, IServiceProvider serviceProvider)
        : base("ReservationStatus", "Estados de reserva", service, serviceProvider)
    {
    }
}