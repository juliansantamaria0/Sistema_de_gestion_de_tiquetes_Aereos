namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;

/// <summary>Caso de uso: Crear tipo de documento.</summary>
public sealed class CreateDocumentTypeUseCase
{
    private readonly IDocumentTypeService _service;
    public CreateDocumentTypeUseCase(IDocumentTypeService service) => _service = service;
    public Task<DocumentTypeDto> ExecuteAsync(CreateDocumentTypeRequest request, CancellationToken ct = default)
        => _service.CreateAsync(request, ct);
}
