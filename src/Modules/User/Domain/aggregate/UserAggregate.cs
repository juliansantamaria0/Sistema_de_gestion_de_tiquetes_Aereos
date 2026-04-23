namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.ValueObject;






public sealed class UserAggregate
{
    public UserId    Id           { get; private set; }
    public int       PersonId     { get; private set; }
    public int       RoleId       { get; private set; }
    public string    Username     { get; private set; } = string.Empty;
    public string    PasswordHash { get; private set; } = string.Empty;
    public bool      IsActive     { get; private set; }
    public DateTime  CreatedAt    { get; private set; }
    public DateTime? UpdatedAt    { get; private set; }

    private UserAggregate() { }

    public static UserAggregate Create(int personId, int roleId, string username, string passwordHash, bool isActive = true)
    {
        Validate(personId, roleId, username, passwordHash);
        return new UserAggregate
        {
            PersonId     = personId,
            RoleId       = roleId,
            Username     = username.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            IsActive     = isActive,
            CreatedAt    = DateTime.UtcNow
        };
    }

    public static UserAggregate Reconstitute(int id, int personId, int roleId, string username,
        string passwordHash, bool isActive, DateTime createdAt, DateTime? updatedAt) =>
        new()
        {
            Id           = UserId.New(id),
            PersonId     = personId,
            RoleId       = roleId,
            Username     = username,
            PasswordHash = passwordHash,
            IsActive     = isActive,
            CreatedAt    = createdAt,
            UpdatedAt    = updatedAt
        };

    public void Update(int roleId, string username, bool isActive)
    {
        if (roleId <= 0)
            throw new ArgumentException("RoleId must be positive.", nameof(roleId));
        ValidateUsername(username);
        RoleId    = roleId;
        Username  = username.Trim().ToLowerInvariant();
        IsActive  = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("PasswordHash cannot be empty.", nameof(newPasswordHash));
        PasswordHash = newPasswordHash;
        UpdatedAt    = DateTime.UtcNow;
    }

    public void Activate()   { IsActive = true;  UpdatedAt = DateTime.UtcNow; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }

    private static void Validate(int personId, int roleId, string username, string passwordHash)
    {
        if (personId <= 0)   throw new ArgumentException("PersonId must be positive.", nameof(personId));
        if (roleId <= 0)     throw new ArgumentException("RoleId must be positive.", nameof(roleId));
        ValidateUsername(username);
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash cannot be empty.", nameof(passwordHash));
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        if (username.Length > 60)
            throw new ArgumentException("Username cannot exceed 60 characters.", nameof(username));
    }
}
