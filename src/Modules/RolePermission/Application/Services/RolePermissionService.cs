namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _repository;
    private readonly IUnitOfWork               _unitOfWork;

    public RolePermissionService(IRolePermissionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RolePermissionDto> CreateAsync(CreateRolePermissionRequest request, CancellationToken ct = default)
    {
        var entity = RolePermissionAggregate.Create(request.RoleId, request.PermissionId);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task<RolePermissionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(RolePermissionId.New(id), ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<RolePermissionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<RolePermissionDto>> GetByRoleIdAsync(int roleId, CancellationToken ct = default)
    {
        var list = await _repository.GetByRoleIdAsync(roleId, ct);
        return list.Select(Map).ToList();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(RolePermissionId.New(id), ct)
            ?? throw new KeyNotFoundException($"RolePermission with id {id} not found.");
        _repository.Delete(entity);
        await _unitOfWork.CommitAsync(ct);
    }

    private static RolePermissionDto Map(RolePermissionAggregate a) =>
        new(a.Id.Value, a.RoleId, a.PermissionId);
}
