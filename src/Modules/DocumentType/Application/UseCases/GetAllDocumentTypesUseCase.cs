namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;

/// <summary>Caso de uso: Obtener todos los tipos de documento.</summary>
public sealed class GetAllDocumentTypesUseCase
{
    private readonly IDocumentTypeService _service;
    public GetAllDocumentTypesUseCase(IDocumentTypeService service) => _service = service;
    public Task<IReadOnlyList<DocumentTypeDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
