
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.AircraftType.UI;

public sealed class AircraftTypeConsoleUI : ReflectiveModuleUI<IAircraftTypeService>
{
    public AircraftTypeConsoleUI(IAircraftTypeService service, IServiceProvider serviceProvider)
        : base("AircraftType", "Tipos de aeronave", service, serviceProvider)
    {
    }
}