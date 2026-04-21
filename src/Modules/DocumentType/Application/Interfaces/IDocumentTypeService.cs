namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;

public interface IDocumentTypeService
{
    Task<DocumentTypeDto>               CreateAsync(CreateDocumentTypeRequest  request, CancellationToken ct = default);
    Task<DocumentTypeDto?>              GetByIdAsync(int id,                            CancellationToken ct = default);
    Task<IReadOnlyList<DocumentTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<DocumentTypeDto>               UpdateAsync(int id, UpdateDocumentTypeRequest request, CancellationToken ct = default);
    Task                                DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record DocumentTypeDto(int DocumentTypeId, string Name, string Code);
public sealed record CreateDocumentTypeRequest(string Name, string Code);
public sealed record UpdateDocumentTypeRequest(string Name, string Code);
