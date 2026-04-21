namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IUnitOfWork     _unitOfWork;

    public RoleService(IRoleRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var role = RoleAggregate.Create(request.Name, request.IsActive);
        await _repository.AddAsync(role, ct);
        await _unitOfWork.CommitAsync(ct);
        return Map(role);
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var role = await _repository.GetByIdAsync(RoleId.New(id), ct);
        return role is null ? null : Map(role);
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<RoleDto> UpdateAsync(int id, UpdateRoleRequest request, CancellationToken ct = default)
    {
        var role = await _repository.GetByIdAsync(RoleId.New(id), ct)
            ?? throw new KeyNotFoundException($"Role with id {id} not found.");
        role.Update(request.Name, request.IsActive);
        _repository.Update(role);
        await _unitOfWork.CommitAsync(ct);
        return Map(role);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var role = await _repository.GetByIdAsync(RoleId.New(id), ct)
            ?? throw new KeyNotFoundException($"Role with id {id} not found.");
        _repository.Delete(role);
        await _unitOfWork.CommitAsync(ct);
    }

    private static RoleDto Map(RoleAggregate r) => new(r.Id.Value, r.Name, r.IsActive);
}
