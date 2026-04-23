namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Services;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork     _unitOfWork;
    private readonly AppDbContext    _context;

    public UserService(IUserRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var normalizedUsername = (request.Username ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalizedUsername))
            throw new InvalidOperationException("El username es obligatorio.");

        if (!await _context.Persons.AsNoTracking().AnyAsync(x => x.Id == request.PersonId, ct))
            throw new InvalidOperationException($"No existe la persona con id {request.PersonId}.");

        if (!await _context.Roles.AsNoTracking().AnyAsync(x => x.RoleId == request.RoleId, ct))
            throw new InvalidOperationException($"No existe el rol con id {request.RoleId}.");

        if (await _context.Users.AsNoTracking().AnyAsync(x => x.PersonId == request.PersonId, ct))
            throw new InvalidOperationException("Ya existe un usuario asociado a esta persona.");

        if (await _context.Users.AsNoTracking().AnyAsync(x => x.Username == normalizedUsername, ct))
            throw new InvalidOperationException("El username ya está en uso.");

        var user = UserAggregate.Create(
            request.PersonId, request.RoleId,
            normalizedUsername, PasswordHasher.Hash(request.Password), request.IsActive);
        await _repository.AddAsync(user, ct);
        await _unitOfWork.CommitAsync(ct);

        var created = await _repository.GetByUsernameAsync(user.Username, ct);
        return created is null ? Map(user) : Map(created);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(UserId.New(id), ct);
        return user is null ? null : Map(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(UserId.New(id), ct)
            ?? throw new KeyNotFoundException($"User with id {id} not found.");
        user.Update(request.RoleId, request.Username, request.IsActive);
        _repository.Update(user);
        await _unitOfWork.CommitAsync(ct);
        return Map(user);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(UserId.New(id), ct)
            ?? throw new KeyNotFoundException($"User with id {id} not found.");
        _repository.Delete(user);
        await _unitOfWork.CommitAsync(ct);
    }

    private static UserDto Map(UserAggregate a) =>
        new(a.Id.Value, a.PersonId, a.RoleId, a.Username, a.IsActive, a.CreatedAt, a.UpdatedAt);
}
