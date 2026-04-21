namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.ValueObject;

/// <summary>
/// Rol de sistema para control de acceso.
/// Ejemplos: ADMIN, OPERATOR, VIEWER, AGENT.
/// Invariante: name normalizado a UPPER_SNAKE_CASE.
/// </summary>
public sealed class RoleAggregate
{
    public RoleId Id       { get; private set; }
    public string Name     { get; private set; } = string.Empty;
    public bool   IsActive { get; private set; }

    private RoleAggregate() { }

    public static RoleAggregate Create(string name, bool isActive = true)
    {
        ValidateName(name);
        return new RoleAggregate
        {
            Name     = name.Trim().ToUpperInvariant(),
            IsActive = isActive
        };
    }

    public static RoleAggregate Reconstitute(int id, string name, bool isActive) =>
        new() { Id = RoleId.New(id), Name = name, IsActive = isActive };

    public void Update(string name, bool isActive)
    {
        ValidateName(name);
        Name     = name.Trim().ToUpperInvariant();
        IsActive = isActive;
    }

    public void Activate()   => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));
        if (name.Length > 50)
            throw new ArgumentException("Role name cannot exceed 50 characters.", nameof(name));
    }
}
