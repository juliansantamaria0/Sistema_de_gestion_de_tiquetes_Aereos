namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public PermissionService(IPermissionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionRequest request, CancellationToken ct = default)
    {
        var entity = PermissionAggregate.Create(request.Name, request.Description);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task<PermissionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(PermissionId.New(id), ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<PermissionDto> UpdateAsync(int id, UpdatePermissionRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(PermissionId.New(id), ct)
            ?? throw new KeyNotFoundException($"Permission with id {id} not found.");
        entity.Update(request.Name, request.Description);
        _repository.Update(entity);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(PermissionId.New(id), ct)
            ?? throw new KeyNotFoundException($"Permission with id {id} not found.");
        _repository.Delete(entity);
        await _unitOfWork.CommitAsync(ct);
    }

    private static PermissionDto Map(PermissionAggregate a) =>
        new(a.Id.Value, a.Name, a.Description);
}
