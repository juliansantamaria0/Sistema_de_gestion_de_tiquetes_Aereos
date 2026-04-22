namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class AircraftManufacturerConsoleUI : ReflectiveModuleUI<IAircraftManufacturerService>
{
    public AircraftManufacturerConsoleUI(IAircraftManufacturerService service, IServiceProvider serviceProvider)
        : base("aircraft_manufacturer", "Aircraft Manufacturer Management", service, serviceProvider)
    {
    }
}
