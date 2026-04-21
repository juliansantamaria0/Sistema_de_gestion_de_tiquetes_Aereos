namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;

/// <summary>Caso de uso: Eliminar tipo de documento.</summary>
public sealed class DeleteDocumentTypeUseCase
{
    private readonly IDocumentTypeService _service;
    public DeleteDocumentTypeUseCase(IDocumentTypeService service) => _service = service;
    public Task ExecuteAsync(int id, CancellationToken ct = default)
        => _service.DeleteAsync(id, ct);
}
