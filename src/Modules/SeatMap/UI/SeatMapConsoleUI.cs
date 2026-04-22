namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class SeatMapConsoleUI : ReflectiveModuleUI<ISeatMapService>
{
    public SeatMapConsoleUI(ISeatMapService service, IServiceProvider serviceProvider)
        : base("seat_map", "Seat Map Management", service, serviceProvider)
    {
    }
}
