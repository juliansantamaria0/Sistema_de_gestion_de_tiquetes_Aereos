using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.ReservationDetail.UI;

public sealed class ReservationDetailConsoleUI : ReflectiveModuleUI<IReservationDetailService>
{
    public ReservationDetailConsoleUI(IReservationDetailService service, IServiceProvider serviceProvider)
        : base("ReservationDetail", "Detalles de reserva", service, serviceProvider)
    {
    }
}