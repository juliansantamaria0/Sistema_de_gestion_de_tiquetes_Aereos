namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;

/// <summary>Caso de uso: Obtener tipo de documento por ID.</summary>
public sealed class GetDocumentTypeByIdUseCase
{
    private readonly IDocumentTypeService _service;
    public GetDocumentTypeByIdUseCase(IDocumentTypeService service) => _service = service;
    public Task<DocumentTypeDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
