using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.DocumentType.UI;

public sealed class DocumentTypeConsoleUI : ReflectiveModuleUI<IDocumentTypeService>
{
    public DocumentTypeConsoleUI(IDocumentTypeService service, IServiceProvider serviceProvider)
        : base("DocumentType", "Tipos de documento", service, serviceProvider)
    {
    }
}