namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class DocumentTypeConsoleUI : ReflectiveModuleUI<IDocumentTypeService>
{
    public DocumentTypeConsoleUI(IDocumentTypeService service, IServiceProvider serviceProvider)
        : base("document_type", "Document Type Management", service, serviceProvider)
    {
    }
}
