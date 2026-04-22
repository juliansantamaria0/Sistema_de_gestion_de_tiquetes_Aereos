namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PersonConsoleUI : ReflectiveModuleUI<IPersonService>
{
    public PersonConsoleUI(IPersonService service, IServiceProvider serviceProvider)
        : base("person", "Person Management", service, serviceProvider)
    {
    }
}
