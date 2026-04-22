namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PassengerConsoleUI : ReflectiveModuleUI<IPassengerService>
{
    public PassengerConsoleUI(IPassengerService service, IServiceProvider serviceProvider)
        : base("passenger", "Passenger Management", service, serviceProvider)
    {
    }
}
