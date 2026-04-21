namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public DocumentTypeService(IDocumentTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeRequest request, CancellationToken ct = default)
    {
        var entity = DocumentTypeAggregate.Create(request.Name, request.Code);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task<DocumentTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(DocumentTypeId.New(id), ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<DocumentTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<DocumentTypeDto> UpdateAsync(int id, UpdateDocumentTypeRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(DocumentTypeId.New(id), ct)
            ?? throw new KeyNotFoundException($"DocumentType with id {id} not found.");
        entity.Update(request.Name, request.Code);
        _repository.Update(entity);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(DocumentTypeId.New(id), ct)
            ?? throw new KeyNotFoundException($"DocumentType with id {id} not found.");
        _repository.Delete(entity);
        await _unitOfWork.CommitAsync(ct);
    }

    private static DocumentTypeDto Map(DocumentTypeAggregate a) =>
        new(a.Id.Value, a.Name, a.Code);
}
