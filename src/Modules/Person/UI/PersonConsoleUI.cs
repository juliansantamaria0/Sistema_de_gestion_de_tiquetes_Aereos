using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.Person.UI;

public sealed class PersonConsoleUI : ReflectiveModuleUI<IPersonService>
{
    public PersonConsoleUI(IPersonService service, IServiceProvider serviceProvider)
        : base("Person", "Personas", service, serviceProvider)
    {
    }
}