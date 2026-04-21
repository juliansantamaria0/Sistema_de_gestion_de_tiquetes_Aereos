namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;

/// <summary>Caso de uso: Actualizar tipo de documento.</summary>
public sealed class UpdateDocumentTypeUseCase
{
    private readonly IDocumentTypeService _service;
    public UpdateDocumentTypeUseCase(IDocumentTypeService service) => _service = service;
    public Task<DocumentTypeDto> ExecuteAsync(int id, UpdateDocumentTypeRequest request, CancellationToken ct = default)
        => _service.UpdateAsync(id, request, ct);
}
