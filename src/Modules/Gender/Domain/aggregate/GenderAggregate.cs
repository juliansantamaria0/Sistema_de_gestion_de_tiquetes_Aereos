namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;

/// <summary>
/// Catálogo de géneros.
/// Valores esperados: MALE, FEMALE, OTHER, PREFER_NOT_TO_SAY.
/// Nombre normalizado a mayúsculas para consistencia con el catálogo SQL.
/// </summary>
public sealed class GenderAggregate
{
    public GenderId Id   { get; private set; }
    public string   Name { get; private set; }

    private GenderAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public GenderAggregate(GenderId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Gender name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "Gender name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Gender name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException(
                "Gender name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
