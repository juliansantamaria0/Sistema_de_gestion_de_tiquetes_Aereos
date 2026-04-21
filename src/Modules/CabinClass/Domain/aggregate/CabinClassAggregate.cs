namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.ValueObject;

/// <summary>
/// Catálogo de clases de cabina.
/// Valores esperados: ECONOMY, BUSINESS, FIRST.
/// Nombre normalizado a mayúsculas para consistencia con el catálogo SQL.
/// </summary>
public sealed class CabinClassAggregate
{
    public CabinClassId Id   { get; private set; }
    public string       Name { get; private set; }

    private CabinClassAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public CabinClassAggregate(CabinClassId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("CabinClass name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "CabinClass name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("CabinClass name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException(
                "CabinClass name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
