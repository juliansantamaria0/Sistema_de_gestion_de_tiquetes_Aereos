namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.ValueObject;

public interface IDocumentTypeRepository
{
    Task<DocumentTypeAggregate?>          GetByIdAsync(DocumentTypeId id, CancellationToken ct = default);
    Task<IReadOnlyList<DocumentTypeAggregate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(DocumentTypeAggregate entity, CancellationToken ct = default);
    void Update(DocumentTypeAggregate entity);
    void Delete(DocumentTypeAggregate entity);
}
