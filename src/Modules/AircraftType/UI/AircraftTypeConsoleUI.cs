namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class AircraftTypeConsoleUI : ReflectiveModuleUI<IAircraftTypeService>
{
    public AircraftTypeConsoleUI(IAircraftTypeService service, IServiceProvider serviceProvider)
        : base("aircraft_type", "Aircraft Type Management", service, serviceProvider)
    {
    }
}
