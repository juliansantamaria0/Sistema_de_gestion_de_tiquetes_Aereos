namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PassengerContactConsoleUI : ReflectiveModuleUI<IPassengerContactService>
{
    public PassengerContactConsoleUI(IPassengerContactService service, IServiceProvider serviceProvider)
        : base("passenger_contact", "Passenger Contact Management", service, serviceProvider)
    {
    }
}
