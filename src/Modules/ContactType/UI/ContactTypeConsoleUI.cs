namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class ContactTypeConsoleUI : ReflectiveModuleUI<IContactTypeService>
{
    public ContactTypeConsoleUI(IContactTypeService service, IServiceProvider serviceProvider)
        : base("contact_type", "Contact Type Management", service, serviceProvider)
    {
    }
}
