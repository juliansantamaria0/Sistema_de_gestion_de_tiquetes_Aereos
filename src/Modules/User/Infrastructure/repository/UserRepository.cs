namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<UserAggregate?> GetByIdAsync(UserId id, CancellationToken ct = default)
    {
        var e = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == id.Value, ct);
        return e is null ? null : ToAggregate(e);
    }

    public async Task<IReadOnlyList<UserAggregate>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _context.Users
            .AsNoTracking()
            .OrderBy(x => x.Username)
            .ToListAsync(ct);
        return list.Select(ToAggregate).ToList();
    }

    public async Task<UserAggregate?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var normalized = username.Trim().ToLowerInvariant();
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == normalized, ct);

        return entity is null ? null : ToAggregate(entity);
    }

    public async Task AddAsync(UserAggregate entity, CancellationToken ct = default)
        => await _context.Users.AddAsync(ToEntity(entity), ct);

    public void Update(UserAggregate entity) => _context.Users.Update(ToEntity(entity));

    public void Delete(UserAggregate entity)
        => _context.Users.Remove(new UserEntity { UserId = entity.Id.Value });

    private static UserAggregate ToAggregate(UserEntity e) =>
        UserAggregate.Reconstitute(e.UserId, e.PersonId, e.RoleId, e.Username,
            e.PasswordHash, e.IsActive, e.CreatedAt, e.UpdatedAt);

    private static UserEntity ToEntity(UserAggregate a) => new()
    {
        UserId       = a.Id.Value,
        PersonId     = a.PersonId,
        RoleId       = a.RoleId,
        Username     = a.Username,
        PasswordHash = a.PasswordHash,
        IsActive     = a.IsActive,
        CreatedAt    = a.CreatedAt,
        UpdatedAt    = a.UpdatedAt
    };
}
