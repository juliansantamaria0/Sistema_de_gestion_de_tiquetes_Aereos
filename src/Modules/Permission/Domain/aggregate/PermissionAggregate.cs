namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.ValueObject;

/// <summary>
/// Permiso de sistema (acción granular).
/// Ejemplos: flights.read, reservations.write, users.admin.
/// </summary>
public sealed class PermissionAggregate
{
    public PermissionId Id          { get; private set; }
    public string       Name        { get; private set; } = string.Empty;
    public string?      Description { get; private set; }

    private PermissionAggregate() { }

    public static PermissionAggregate Create(string name, string? description = null)
    {
        ValidateName(name);
        return new PermissionAggregate
        {
            Name        = name.Trim().ToLowerInvariant(),
            Description = description?.Trim()
        };
    }

    public static PermissionAggregate Reconstitute(int id, string name, string? description) =>
        new() { Id = PermissionId.New(id), Name = name, Description = description };

    public void Update(string name, string? description)
    {
        ValidateName(name);
        Name        = name.Trim().ToLowerInvariant();
        Description = description?.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty.", nameof(name));
        if (name.Length > 80)
            throw new ArgumentException("Permission name cannot exceed 80 characters.", nameof(name));
    }
}
