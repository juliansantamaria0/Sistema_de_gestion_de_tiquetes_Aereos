namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PassengerDiscountConsoleUI : ReflectiveModuleUI<IPassengerDiscountService>
{
    public PassengerDiscountConsoleUI(IPassengerDiscountService service, IServiceProvider serviceProvider)
        : base("passenger_discount", "Passenger Discount Management", service, serviceProvider)
    {
    }
}
