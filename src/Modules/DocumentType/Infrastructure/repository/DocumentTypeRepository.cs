namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly AppDbContext _context;
    public DocumentTypeRepository(AppDbContext context) => _context = context;

    public async Task<DocumentTypeAggregate?> GetByIdAsync(DocumentTypeId id, CancellationToken ct = default)
    {
        var e = await _context.DocumentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DocumentTypeId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<DocumentTypeAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.DocumentTypes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task AddAsync(DocumentTypeAggregate entity, CancellationToken ct = default)
        => await _context.DocumentTypes.AddAsync(ToEntity(entity), ct);

    public void Update(DocumentTypeAggregate entity)
        => _context.DocumentTypes.Update(ToEntity(entity));

    public void Delete(DocumentTypeAggregate entity)
        => _context.DocumentTypes.Remove(new DocumentTypeEntity { DocumentTypeId = entity.Id.Value });

    private static DocumentTypeAggregate ToAggregate(DocumentTypeEntity e) =>
        DocumentTypeAggregate.Reconstitute(e.DocumentTypeId, e.Name, e.Code);

    private static DocumentTypeEntity ToEntity(DocumentTypeAggregate a) =>
        new() { DocumentTypeId = a.Id.Value, Name = a.Name, Code = a.Code };
}
